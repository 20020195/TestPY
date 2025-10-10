import {
    HandLandmarker,
    FilesetResolver
} from "https://cdn.jsdelivr.net/npm/@mediapipe/tasks-vision@0.10.17";

let handLandmarker = undefined;
let runningMode = "VIDEO";
let webcamRunning = false;

const video = document.getElementById("webcam");

const hasGetUserMedia = () => !!navigator.mediaDevices?.getUserMedia;

const createHandLandmarker = async () => {
    const vision = await FilesetResolver.forVisionTasks(
        "https://cdn.jsdelivr.net/npm/@mediapipe/tasks-vision@0.10.17/wasm"
    );
    handLandmarker = await HandLandmarker.createFromOptions(vision, {
        baseOptions: {
            modelAssetPath: `https://storage.googleapis.com/mediapipe-models/hand_landmarker/hand_landmarker/float16/1/hand_landmarker.task`,
            delegate: "GPU"
        },
        runningMode: runningMode,
        numHands: 1
    });
    if (hasGetUserMedia()) {
        enableCam();
    }
};

createHandLandmarker();

function enableCam(event) {
    if (webcamRunning === true) {
        webcamRunning = false;
    } else {
        webcamRunning = true;
    }

    const savedCameraId = localStorage.getItem("defaultCameraId");
    const constraints = {
        video: savedCameraId ? { deviceId: { exact: savedCameraId } } : true,
    };

    navigator.mediaDevices
        .getUserMedia(constraints)
        .then(function (stream) {
            video.srcObject = stream;
            video.addEventListener("loadeddata", predictWebcam);
        })
        .catch((error) => {
            console.error("Error accessing the camera: ", error);
            navigator.mediaDevices
                .getUserMedia({ video: true })
                .then(function (stream) {
                    video.srcObject = stream;
                    video.addEventListener("loadeddata", predictWebcam);
                });
        });
}

let lastVideoTime = -1;
let results;
const handleResultLandmarks1Hands = (results) => {
    for (const landmarks of results.landmarks) {
        const jsonString = JSON.stringify(landmarks);
        window.unityExports?.unityInstance.SendMessage(
            "InputFromJs",
            "GetRightHandPos",
            jsonString
        );
    }
};

let isSended = false;

async function predictWebcam() {
    video.style.height = "180px";
    video.style.width = "240px";

    if (window.unityExports != undefined && !isSended) {
        window.unityExports.unityInstance.SendMessage("InputFromJs", "ConnectedCameraStatus", "")
        isSended = true;
    }

    let startTimeMs = performance.now();
    if (lastVideoTime !== video.currentTime) {
        lastVideoTime = video.currentTime;

        let canvas;

        canvas = document.createElement('canvas');
        const ctx = canvas.getContext('2d');

        canvas.width = video.videoWidth;
        canvas.height = video.videoHeight;

        ctx.drawImage(video, 0, 0);

        if (
            localStorage.getItem(
                `cameraFlipState_${localStorage.getItem("defaultCameraId")}`
            ) === "true"
        ) {
            ctx.translate(canvas.width, 0);
            ctx.scale(-1, 1);
            ctx.drawImage(video, 0, 0);
        }
        results = handLandmarker.detectForVideo(canvas, startTimeMs);
    }

    if (results.landmarks) {
        handleResultLandmarks1Hands(results);
    }

    if (webcamRunning === true) {
        window.requestAnimationFrame(predictWebcam);
    } else {
        video.srcObject.getTracks().forEach((track) => track.stop());
        video.srcObject = null;
    }
}