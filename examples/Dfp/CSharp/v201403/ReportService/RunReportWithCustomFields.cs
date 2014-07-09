// Copyright 2014, Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// Author: api.anash@gmail.com (Anash P. Oommen)

using Google.Api.Ads.Dfp.Lib;
using Google.Api.Ads.Dfp.v201403;
using Google.Api.Ads.Dfp.Util.v201403;

using System;
using System.Threading;
using System.Collections.Generic;

namespace Google.Api.Ads.Dfp.Examples.CSharp.v201403 {
  /// <summary>
  /// This code example runs a report that includes custom fields found in the
  /// line items of an order. To download the report see DownloadReport.cs.
  ///
  /// Tag: ReportService.runReportJob
  /// Tag: ReportService.getReportJob
  /// Tag: LineItemService.getLineItemsByStatement
  /// </summary>
  class RunReportWithCustomFields : SampleBase {
    /// <summary>
    /// Returns a description about the code example.
    /// </summary>
    public override string Description {
      get {
        return "This code example runs a report that includes custom fields found in the " +
            "line items of an order. To download the report see DownloadReport.cs.";
      }
    }

    /// <summary>
    /// Main method, to run this code example as a standalone application.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    public static void Main(string[] args) {
      SampleBase codeExample = new RunReportWithCustomFields();
      Console.WriteLine(codeExample.Description);
      codeExample.Run(new DfpUser());
    }

    /// <summary>
    /// Run the code example.
    /// </summary>
    /// <param name="user">The DFP user object running the code example.</param>
    public override void Run(DfpUser user) {
      // Get the LineItemService.
      LineItemService lineItemService =
          (LineItemService) user.GetService(DfpService.v201403.LineItemService);
      // Get the ReportService.
      ReportService reportService =
          (ReportService) user.GetService(DfpService.v201403.ReportService);

      try {
        // Set the ID of the order to get line items from.
        long orderId = long.Parse(_T("INSERT_ORDER_ID_HERE"));

        // Sets default for page.
        LineItemPage page = new LineItemPage();

        // Create a statement to only select line items from a given order.
        StatementBuilder statementBuilder = new StatementBuilder()
            .Where("orderId = :orderId")
            .Limit(StatementBuilder.SUGGESTED_PAGE_LIMIT)
            .AddValue("orderId", orderId);


        // Collect all line item custom field IDs for an order.
        List<long> customFieldIds = new List<long>();
        do {
          // Get line items by statement.
          page = lineItemService.getLineItemsByStatement(statementBuilder.ToStatement());

          // Get custom field IDs from the line items of an order.
          if (page.results != null) {
            foreach (LineItem lineItem in page.results) {
              if (lineItem.customFieldValues != null) {
                foreach (BaseCustomFieldValue customFieldValue in lineItem.customFieldValues) {
                  if (!customFieldIds.Contains(customFieldValue.customFieldId)) {
                    customFieldIds.Add(customFieldValue.customFieldId);
                  }
                }
              }
            }
          }

          statementBuilder.IncreaseOffsetBy(StatementBuilder.SUGGESTED_PAGE_LIMIT);
        } while (statementBuilder.GetOffset() < page.totalResultSetSize);


        // Create statement to filter for an order.
        statementBuilder.RemoveLimitAndOffset();

        // Create report job.
        ReportJob reportJob = new ReportJob();

        // Create report query.
        ReportQuery reportQuery = new ReportQuery();
        reportQuery.dateRangeType = DateRangeType.LAST_MONTH;
        reportQuery.dimensions = new Dimension[] {Dimension.LINE_ITEM_ID, Dimension.LINE_ITEM_NAME};
        reportQuery.statement = statementBuilder.ToStatement();
        reportQuery.customFieldIds = customFieldIds.ToArray();
        reportQuery.columns = new Column[] {Column.AD_SERVER_IMPRESSIONS};
        reportJob.reportQuery = reportQuery;

        // Run report job.
        reportJob = reportService.runReportJob(reportJob);

        do {
          Console.WriteLine("Report with ID '{0}' is still running.", reportJob.id);
          Thread.Sleep(30000);
          // Get report job.
          reportJob = reportService.getReportJob(reportJob.id);
        } while (reportJob.reportJobStatus == ReportJobStatus.IN_PROGRESS);

        if (reportJob.reportJobStatus == ReportJobStatus.FAILED) {
          Console.WriteLine("Report job with ID '{0}' failed to finish successfully.",
              reportJob.id);
        } else {
          Console.WriteLine("Report job with ID '{0}' completed successfully.", reportJob.id);
        }
      } catch (Exception ex) {
        Console.WriteLine("Failed to run cusom fields report. Exception says \"{0}\"",
            ex.Message);
      }
    }
  }
}
