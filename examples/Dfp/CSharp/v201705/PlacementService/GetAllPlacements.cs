// Copyright 2017, Google Inc. All Rights Reserved.
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
using Google.Api.Ads.Dfp.Util.v201705;
using Google.Api.Ads.Dfp.v201705;
using System;

namespace Google.Api.Ads.Dfp.Examples.CSharp.v201705 {
  /// <summary>
  /// This example gets all placements.
  /// </summary>
  public class GetAllPlacements : SampleBase {
    /// <summary>
    /// Returns a description about the code example.
    /// </summary>
    public override string Description {
      get {
        return "This example gets all placements.";
      }
    }

    /// <summary>
    /// Main method, to run this code example as a standalone application.
    /// </summary>
    public static void Main() {
      GetAllPlacements codeExample = new GetAllPlacements();
      Console.WriteLine(codeExample.Description);
      try {
        codeExample.Run(new DfpUser());
      } catch (Exception e) {
        Console.WriteLine("Failed to get placements. Exception says \"{0}\"",
            e.Message);
      }
    }

    /// <summary>
    /// Run the code example.
    /// </summary>
    public void Run(DfpUser dfpUser) {
      using (PlacementService placementService =
          (PlacementService) dfpUser.GetService(DfpService.v201705.PlacementService)) {

        // Create a statement to select placements.
        int pageSize = StatementBuilder.SUGGESTED_PAGE_LIMIT;
        StatementBuilder statementBuilder = new StatementBuilder()
            .OrderBy("id ASC")
            .Limit(pageSize);

        // Retrieve a small amount of placements at a time, paging through until all
        // placements have been retrieved.
        int totalResultSetSize = 0;
        do {
          PlacementPage page = placementService.getPlacementsByStatement(
              statementBuilder.ToStatement());

          // Print out some information for each placement.
          if (page.results != null) {
            totalResultSetSize = page.totalResultSetSize;
            int i = page.startIndex;
            foreach (Placement placement in page.results) {
              Console.WriteLine(
                  "{0}) Placement with ID {1} and name \"{2}\" was found.",
                  i++,
                  placement.id,
                  placement.name
              );
            }
          }

          statementBuilder.IncreaseOffsetBy(pageSize);
        } while (statementBuilder.GetOffset() < totalResultSetSize);

        Console.WriteLine("Number of results found: {0}", totalResultSetSize);
      }
    }
  }
}
