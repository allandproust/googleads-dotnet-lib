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
  /// This example gets all audience segments.
  /// </summary>
  public class GetAllAudienceSegments : SampleBase {
    /// <summary>
    /// Returns a description about the code example.
    /// </summary>
    public override string Description {
      get {
        return "This example gets all audience segments.";
      }
    }

    /// <summary>
    /// Main method, to run this code example as a standalone application.
    /// </summary>
    public static void Main() {
      GetAllAudienceSegments codeExample = new GetAllAudienceSegments();
      Console.WriteLine(codeExample.Description);
      try {
        codeExample.Run(new DfpUser());
      } catch (Exception e) {
        Console.WriteLine("Failed to get audience segments. Exception says \"{0}\"",
            e.Message);
      }
    }

    /// <summary>
    /// Run the code example.
    /// </summary>
    public void Run(DfpUser dfpUser) {
      using (AudienceSegmentService audienceSegmentService =
          (AudienceSegmentService) dfpUser.GetService(DfpService.v201705.AudienceSegmentService)) {

        // Create a statement to select audience segments.
        int pageSize = StatementBuilder.SUGGESTED_PAGE_LIMIT;
        StatementBuilder statementBuilder = new StatementBuilder()
            .OrderBy("id ASC")
            .Limit(pageSize);

        // Retrieve a small amount of audience segments at a time, paging through until all
        // audience segments have been retrieved.
        int totalResultSetSize = 0;
        do {
          AudienceSegmentPage page = audienceSegmentService.getAudienceSegmentsByStatement(
              statementBuilder.ToStatement());

          // Print out some information for each audience segment.
          if (page.results != null) {
            totalResultSetSize = page.totalResultSetSize;
            int i = page.startIndex;
            foreach (AudienceSegment audienceSegment in page.results) {
              Console.WriteLine(
                  "{0}) Audience segment with ID {1}, name \"{2}\", and size {3} was found.",
                  i++,
                  audienceSegment.id,
                  audienceSegment.name,
                  audienceSegment.size
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
