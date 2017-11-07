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
  /// This example gets all active activities.
  /// </summary>
  public class GetActiveActivities : SampleBase {
    /// <summary>
    /// Returns a description about the code example.
    /// </summary>
    public override string Description {
      get {
        return "This example gets all active activities.";
      }
    }

    /// <summary>
    /// Main method, to run this code example as a standalone application.
    /// </summary>
    public static void Main() {
      GetActiveActivities codeExample = new GetActiveActivities();
      Console.WriteLine(codeExample.Description);
      try {
        codeExample.Run(new DfpUser());
      } catch (Exception e) {
        Console.WriteLine("Failed to get activities. Exception says \"{0}\"",
            e.Message);
      }
    }

    /// <summary>
    /// Run the code example.
    /// </summary>
    public void Run(DfpUser dfpUser) {
      using (ActivityService activityService =
          (ActivityService) dfpUser.GetService(DfpService.v201705.ActivityService)) {

        // Create a statement to select activities.
        int pageSize = StatementBuilder.SUGGESTED_PAGE_LIMIT;
        StatementBuilder statementBuilder = new StatementBuilder()
            .Where("status = :status")
            .OrderBy("id ASC")
            .Limit(pageSize)
            .AddValue("status", ActivityStatus.ACTIVE.ToString());

        // Retrieve a small amount of activities at a time, paging through until all
        // activities have been retrieved.
        int totalResultSetSize = 0;
        do {
          ActivityPage page = activityService.getActivitiesByStatement(
              statementBuilder.ToStatement());

          // Print out some information for each activity.
          if (page.results != null) {
            totalResultSetSize = page.totalResultSetSize;
            int i = page.startIndex;
            foreach (Activity activity in page.results) {
              Console.WriteLine(
                  "{0}) Activity with ID {1}, name \"{2}\", and type \"{3}\" was found.",
                  i++,
                  activity.id,
                  activity.name,
                  activity.type
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
