# Developing Spidey Senses : Anomaly detection for IoT apps

Anomaly detection is the process of identifying unexpected items or events in data sets. It’s about detecting the deviation from expected pattern of a dataset. It’s like having “spidey senses” for your apps that can detect when there’s danger or something is not right. Attend this session and learn about using anomaly detection in ML.NET, Azure Stream Analytics and Cognitive Services API, become a superhero and save the day.

## Read more at:
[Hackster Project Page](https://www.hackster.io/RONDAGDAG/spidey-senses-anomaly-detection-f477e7)

## Here's the presentation slides
[Presentation](SpideySense-Anomaly.pdf)

## ML.NET

Open in VS Code with [Remote Containers](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.remote-containers)

Use project
mlnet\TempHumidityAnomalyDetection.csproj

It should build the dev container

## Azure Stream Analytics
Open in VS  Code with [Azure Stream Analytics Tools](https://marketplace.visualstudio.com/items?itemName=ms-bigdatatools.vscode-asa) Installed

Use this workspace
spideySenseASAProj\spideySenseASA.code-workspace

Follow instructions on this [tutorial](https://docs.microsoft.com/en-us/azure/stream-analytics/quick-create-vs-code)

Open browser to [Raspberry PI Azure IoT Online Simulator](https://azure-samples.github.io/raspberry-pi-web-simulator/)
- copy and paste rpi-node every5sec.js to code window
- replace connectionString

Define Transformation Query using spideySenseASAProj.asaql instead of this [query](https://docs.microsoft.com/en-us/azure/stream-analytics/quick-create-vs-code#define-the-transformation-query)

Define live input and live output

## Cognitive Services Anomaly Detector

Click on the link to run in binder
[![Binder](https://mybinder.org/badge_logo.svg)](https://mybinder.org/v2/gh/rondagdag/spideysense-anomaly/master?filepath=AnomalyDetector%2FBatchAnomalyDetectorAPI.ipynb)