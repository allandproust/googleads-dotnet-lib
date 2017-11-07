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
  /// This example gets all products created from a product template.
  /// </summary>
  public class GetProductsForProductTemplate : SampleBase {
    /// <summary>
    /// Returns a description about the code example.
    /// </summary>
    public override string Description {
      get {
        return "This example gets all products created from a product template.";
      }
    }

    /// <summary>
    /// Main method, to run this code example as a standalone application.
    /// </summary>
    public static void Main() {
      GetProductsForProductTemplate codeExample = new GetProductsForProductTemplate();
      long productTemplateId = long.Parse("INSERT_PRODUCT_TEMPLATE_ID_HERE");
      Console.WriteLine(codeExample.Description);
      try {
        codeExample.Run(new DfpUser(), productTemplateId);
      } catch (Exception e) {
        Console.WriteLine("Failed to get products. Exception says \"{0}\"",
            e.Message);
      }
    }

    /// <summary>
    /// Run the code example.
    /// </summary>
    public void Run(DfpUser dfpUser, long productTemplateId) {
      using (ProductService productService =
          (ProductService) dfpUser.GetService(DfpService.v201702.ProductService)) {

        // Create a statement to select products.
        int pageSize = StatementBuilder.SUGGESTED_PAGE_LIMIT;
        StatementBuilder statementBuilder = new StatementBuilder()
            .Where("productTemplateId = :productTemplateId")
            .OrderBy("id ASC")
            .Limit(pageSize)
            .AddValue("productTemplateId", productTemplateId);

        // Retrieve a small amount of products at a time, paging through until all
        // products have been retrieved.
        int totalResultSetSize = 0;
        do {
          ProductPage page = productService.getProductsByStatement(
              statementBuilder.ToStatement());

          // Print out some information for each product.
          if (page.results != null) {
            totalResultSetSize = page.totalResultSetSize;
            int i = page.startIndex;
            foreach (Product product in page.results) {
              Console.WriteLine(
                  "{0}) Product with ID {1} and name \"{2}\" was found.",
                  i++,
                  product.id,
                  product.name
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
