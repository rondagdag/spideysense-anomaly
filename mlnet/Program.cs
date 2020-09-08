using System;
using System.IO;
using Microsoft.ML;
using System.Collections.Generic;
using Microsoft.ML.Data;

namespace TempHumidityAnomalyDetection
{
    public class InitialTempHumidityData
    {
        //"messageId","deviceId","temperature","humidity","enqueuedTimeUtc"
        [LoadColumn(2)]
        public string InputTemperature;
        
        [LoadColumn(3)]
        public string InputHumidity;

        [LoadColumn(4)]
        public string InputEnqueuedTimeUtc;

    }

    // <SnippetDeclareTypes>
    public class TempHumidityData : InitialTempHumidityData
    {

        public float Temperature;
        
        public float Humidity;

    }

    public class TempHumidityPrediction
    {
        //vector to hold alert,score,p-value values
        [VectorType(3)]
        public double[] Prediction { get; set; }
    }

    class Program
    {
        // <SnippetDeclareGlobalVariables>
        static readonly string _dataPath = Path.Combine(Environment.CurrentDirectory, "Data", "testdata.csv");
        // </SnippetDeclareGlobalVariables>
        static void Main(string[] args)
        {
            // Create MLContext to be shared across the model creation workflow objects
            MLContext mlContext = new MLContext();

            //STEP 1: Common data loading configuration
            IDataView data = mlContext.Data.LoadFromTextFile<InitialTempHumidityData>(
                path: _dataPath, hasHeader: true, 
                separatorChar: ',', allowQuoting : true);

            // Construct the pipeline.
            var pipeline = mlContext.Transforms.Conversion.ConvertType(new[]
            {
                    new InputOutputColumnPair("Temperature", nameof(InitialTempHumidityData.InputTemperature)),
                    new InputOutputColumnPair("Humidity", nameof(InitialTempHumidityData.InputHumidity)),
             }, DataKind.Single);

            var transformer = pipeline.Fit(data);
            // Transforming the same data. This will add the 2 columns defined in
            // the pipeline, containing the converted
            // values of the initial columns. 
            var transformedData = transformer.Transform(data);

            // var convertedData = mlContext.Data.CreateEnumerable<TempHumidityData>(
            //     transformedData, true);

            // Spike detects pattern temporary changes
            DetectSpike(mlContext, transformedData);

            // Changepoint detects pattern persistent changes
            // DetectChangepoint(mlContext, transformedData);
        }
        static void DetectSpike(MLContext mlContext, IDataView tempHumidityData)
        {
            Console.WriteLine("Detect temporary changes in pattern");

            // STEP 2: Set the training algorithm
            var iidSpikeEstimator = mlContext.Transforms.DetectIidSpike(
                outputColumnName: nameof(TempHumidityPrediction.Prediction),
                inputColumnName: nameof(TempHumidityData.Temperature), 
                confidence: 95, pvalueHistoryLength: 25 );

            // STEP 3: Create the transform
            // Create the spike detection transform
            Console.WriteLine("=============== Training the model ===============");
            ITransformer iidSpikeTransform = iidSpikeEstimator.Fit(CreateEmptyDataView(mlContext));

            Console.WriteLine("=============== End of training process ===============");
            //Apply data transformation to create predictions.
            IDataView transformedData = iidSpikeTransform.Transform(tempHumidityData);

            var predictions = mlContext.Data.CreateEnumerable<TempHumidityPrediction>(
                transformedData, reuseRowObject: false);

            Console.WriteLine("Alert\tScore\tP-Value");

            foreach (var p in predictions)
            {
                var results = $"{p.Prediction[0]}\t{p.Prediction[1]:f2}\t{p.Prediction[2]:F2}";

                if (p.Prediction[0] == 1)
                {
                    results += " <-- Spike detected";
                }

                Console.WriteLine(results);
            }
            Console.WriteLine("");
        }

        static void DetectChangepoint(MLContext mlContext, IDataView tempHumidityData)
        {
            Console.WriteLine("Detect Persistent changes in pattern");

            //STEP 2: Set the training algorithm
            var iidChangePointEstimator = mlContext.Transforms.DetectIidChangePoint(
                outputColumnName: nameof(TempHumidityPrediction.Prediction), 
                inputColumnName: nameof(TempHumidityData.Temperature), 
                confidence: 90, 
                changeHistoryLength: 15);

            //STEP 3: Create the transform
            Console.WriteLine("=============== Training the model Using Change Point Detection Algorithm===============");
            var iidChangePointTransform = iidChangePointEstimator.Fit(CreateEmptyDataView(mlContext));
            Console.WriteLine("=============== End of training process ===============");

            //Apply data transformation to create predictions.
            IDataView transformedData = iidChangePointTransform.Transform(tempHumidityData);

            var predictions = mlContext.Data.CreateEnumerable<TempHumidityPrediction>(transformedData, reuseRowObject: false);

            Console.WriteLine("Alert\tScore\tP-Value\tMartingale value");

            foreach (var p in predictions)
            {
                var results = $"{p.Prediction[0]}\t{p.Prediction[1]:f2}\t{p.Prediction[2]:F2}\t{p.Prediction[3]:F2}";

                if (p.Prediction[0] == 1)
                {
                    results += " <-- alert is on, predicted changepoint";
                }
                Console.WriteLine(results);
            }
            Console.WriteLine("");
        }

        static IDataView CreateEmptyDataView(MLContext mlContext)
        {
            // Create empty DataView. We just need the schema to call Fit() for the time series transforms
            IEnumerable<TempHumidityData> enumerableData = new List<TempHumidityData>();
            return mlContext.Data.LoadFromEnumerable(enumerableData);
        }
    }
}
