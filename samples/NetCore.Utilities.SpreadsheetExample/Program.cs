﻿using System;
using System.Collections.Generic;
using ICG.NetCore.Utilities.Spreadsheet;
using Microsoft.Extensions.DependencyInjection;
using NetCore.Utilities.SpreadsheetExample.Models;

namespace NetCore.Utilities.SpreadsheetExample
{
    class Program
    {
        static void Main(string[] args)
        {
            //Setup our DI Container
            var services = new ServiceCollection();
            services.UseIcgNetCoreUtilitiesSpreadsheet();
            var provider = services.BuildServiceProvider();

            //Get our generator and export
            var exportGenerator = provider.GetRequiredService<ISpreadsheetGenerator>();
            var exportDefinition = new SpreadsheetConfiguration<SimpleExportData>
            {
                RenderTitle = true,
                DocumentTitle = "Sample Export of 100 Records",
                RenderSubTitle = true,
                DocumentSubTitle = "Showing the full options",
                ExportData = GetSampleExportData(100),
                WorksheetName = "Sample",
                FreezeHeaders = true
            };
            var fileContent = exportGenerator.CreateSingleSheetSpreadsheet(exportDefinition);
            System.IO.File.WriteAllBytes("Sample.xlsx", fileContent);

            //Sample 2 sheet export
            var multiSheetDefinition = new MultisheetConfiguration()
                .WithSheet("Sheet 1", GetSampleExportData(100))
                .WithSheet("Additional Sheet", GetSampleExportData(500), config =>
                {
                    config.DocumentTitle = "Lots of data";
                    config.RenderTitle = true;
                    config.FreezeHeaders = true;
                });

            var multiFileContent = exportGenerator.CreateMultiSheetSpreadsheet(multiSheetDefinition);
            System.IO.File.WriteAllBytes("Sample-Multi.xlsx", multiFileContent);
            Console.WriteLine("Files Created");
            Console.ReadLine();
        }

        private static List<SimpleExportData> GetSampleExportData(int numberOfRecords)
        {
            var listData = new List<SimpleExportData>();
            for (var i = 0; i < numberOfRecords; i++)
            {
                listData.Add(new SimpleExportData
                    {DueDate = DateTime.Now.AddDays(i), Notes = $"Record {i} notes", TotalCost = 15m, Title = $"Sample Data Row #{i}"});
            }

            return listData;
        }
    }
}
