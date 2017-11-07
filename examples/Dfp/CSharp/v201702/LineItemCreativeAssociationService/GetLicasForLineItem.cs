// Copyright 2016, Google Inc. All Rights Reserved.
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
using Google.Api.Ads.Dfp.Lib;
using Google.Api.Ads.Dfp.Util.v201702;
using Google.Api.Ads.Dfp.v201702;
using System;

namespace Google.Api.Ads.Dfp.Examples.CSharp.v201702 {
  /// <summary>
  /// This example gets all line item creative associations for a given line item.
  /// </summary>
  public class GetLicasForLineItem : SampleBase {
    /// <summary>
    /// Returns a description about the code example.
    /// </summary>
    public override string Description {
      get {
        return "This example gets all line item creative associations for a given line item.";
      }
    }

    /// <summary>
    /// Main method, to run this code example as a standalone application.
    /// </summary>
    public static void Main() {
      GetLicasForLineItem codeExample = new GetLicasForLineItem();
      long lineItemId = long.Parse("INSERT_LINE_ITEM_ID_HERE");
      Console.WriteLine(codeExample.Description);
      try {
        codeExample.Run(new DfpUser(), lineItemId);
      } catch (Exception e) {
        Console.WriteLine("Failed to get line item creative associations. Exception says \"{0}\"",
            e.Message);
      }
    }

    /// <summary>
    /// Run the code example.
    /// </summary>
    public void Run(DfpUser dfpUser, long lineItemId) {
      using (LineItemCreativeAssociationService lineItemCreativeAssociationService =
          (LineItemCreativeAssociationService) dfpUser.GetService(
              DfpService.v201702.LineItemCreativeAssociationService)) {

        // Create a statement to select line item creative associations.
        int pageSize = StatementBuilder.SUGGESTED_PAGE_LIMIT;
        StatementBuilder statementBuilder = new StatementBuilder()
            .Where("lineItemId = :lineItemId")
            .OrderBy("lineItemId ASC, creativeId ASC")
            .Limit(pageSize)
            .AddValue("lineItemId", lineItemId);

        // Retrieve a small amount of line item creative associations at a time, paging through
        // until all line item creative associations have been retrieved.
        int totalResultSetSize = 0;
        do {
          LineItemCreativeAssociationPage page =
              lineItemCreativeAssociationService.getLineItemCreativeAssociationsByStatement(
                  statementBuilder.ToStatement());

          // Print out some information for each line item creative association.
          if (page.results != null) {
            totalResultSetSize = page.totalResultSetSize;
            int i = page.startIndex;
            foreach (LineItemCreativeAssociation lica in page.results) {
              if (lica.creativeSetId != 0) {
                Console.WriteLine(
                    "{0}) Line item creative association with line item ID {1} " +
                        "and creative set ID {2} was found.",
                    i++,
                    lica.lineItemId,
                    lica.creativeSetId
                );
              } else {
                Console.WriteLine(
                    "{0}) Line item creative association with line item ID {1} " +
                        "and creative ID {2} was found.",
                    i++,
                    lica.lineItemId,
                    lica.creativeId
                );
              }
            }
          }

          statementBuilder.IncreaseOffsetBy(pageSize);
        } while (statementBuilder.GetOffset() < totalResultSetSize);

        Console.WriteLine("Number of results found: {0}", totalResultSetSize);
      }
    }
  }
}
