' Copyright 2014, Google Inc. All Rights Reserved.
'
' Licensed under the Apache License, Version 2.0 (the "License");
' you may not use this file except in compliance with the License.
' You may obtain a copy of the License at
'
'     http://www.apache.org/licenses/LICENSE-2.0
'
' Unless required by applicable law or agreed to in writing, software
' distributed under the License is distributed on an "AS IS" BASIS,
' WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
' See the License for the specific language governing permissions and
' limitations under the License.

' Author: api.anash@gmail.com (Anash P. Oommen)

Imports Google.Api.Ads.AdWords.Lib
Imports Google.Api.Ads.AdWords.v201402

Imports System
Imports System.Collections.Generic
Imports System.IO

Namespace Google.Api.Ads.AdWords.Examples.VB.v201402
  ''' <summary>
  ''' This code example gets keyword traffic estimates.
  '''
  ''' Tags: TrafficEstimatorService.get
  ''' </summary>
  Public Class EstimateKeywordTraffic
    Inherits ExampleBase
    ''' <summary>
    ''' Main method, to run this code example as a standalone application.
    ''' </summary>
    ''' <param name="args">The command line arguments.</param>
    Public Shared Sub Main(ByVal args As String())
      Dim codeExample As New EstimateKeywordTraffic
      Console.WriteLine(codeExample.Description)
      Try
        codeExample.Run(New AdWordsUser)
      Catch ex As Exception
        Console.WriteLine("An exception occurred while running this code example. {0}", _
            ExampleUtilities.FormatException(ex))
      End Try
    End Sub

    ''' <summary>
    ''' Returns a description about the code example.
    ''' </summary>
    Public Overrides ReadOnly Property Description() As String
      Get
        Return "This code example gets keyword traffic estimates."
      End Get
    End Property

    ''' <summary>
    ''' Runs the code example.
    ''' </summary>
    ''' <param name="user">The AdWords user.</param>
    Public Sub Run(ByVal user As AdWordsUser)
      ' Get the TrafficEstimatorService.
      Dim trafficEstimatorService As TrafficEstimatorService = user.GetService( _
          AdWordsService.v201402.TrafficEstimatorService)

      ' Create keywords. Up to 2000 keywords can be passed in a single request.
      Dim keyword1 As New Keyword
      keyword1.text = "mars cruise"
      keyword1.matchType = KeywordMatchType.BROAD

      Dim keyword2 As New Keyword
      keyword2.text = "cheap cruise"
      keyword2.matchType = KeywordMatchType.PHRASE

      Dim keyword3 As New Keyword
      keyword3.text = "cruise"
      keyword3.matchType = KeywordMatchType.EXACT

      Dim keywords As Keyword() = New Keyword() {keyword1, keyword2, keyword3}

      ' Create a keyword estimate request for each keyword.
      Dim keywordEstimateRequests As New List(Of KeywordEstimateRequest)

      For Each keyword As Keyword In keywords
        Dim keywordEstimateRequest As New KeywordEstimateRequest
        keywordEstimateRequest.keyword = keyword
        keywordEstimateRequests.Add(keywordEstimateRequest)
      Next

      ' Create negative keywords.
      Dim negativeKeyword1 As New Keyword
      negativeKeyword1.text = "moon walk"
      negativeKeyword1.matchType = KeywordMatchType.BROAD

      Dim negativeKeywordEstimateRequest As New KeywordEstimateRequest
      negativeKeywordEstimateRequest.keyword = negativeKeyword1
      negativeKeywordEstimateRequest.isNegative = True
      keywordEstimateRequests.Add(negativeKeywordEstimateRequest)

      ' Create ad group estimate requests.
      Dim adGroupEstimateRequest As New AdGroupEstimateRequest
      adGroupEstimateRequest.keywordEstimateRequests = keywordEstimateRequests.ToArray
      adGroupEstimateRequest.maxCpc = New Money
      adGroupEstimateRequest.maxCpc.microAmount = 1000000

      ' Create campaign estimate requests.
      Dim campaignEstimateRequest As New CampaignEstimateRequest
      campaignEstimateRequest.adGroupEstimateRequests = New AdGroupEstimateRequest() _
          {adGroupEstimateRequest}

      ' See http://code.google.com/apis/adwords/docs/appendix/countrycodes.html
      ' for a detailed list of country codes.
      Dim countryCriterion As New Location
      countryCriterion.id = 2840 'US

      ' See http://code.google.com/apis/adwords/docs/appendix/languagecodes.html
      ' for a detailed list of language codes.
      Dim languageCriterion As New Language
      languageCriterion.id = 1000 'en

      campaignEstimateRequest.criteria = New Criterion() {countryCriterion, languageCriterion}

      ' Create the selector.
      Dim selector As New TrafficEstimatorSelector
      selector.campaignEstimateRequests = New CampaignEstimateRequest() {campaignEstimateRequest}

      Try
        ' Get traffic estimates.
        Dim result As TrafficEstimatorResult = trafficEstimatorService.get(selector)

        ' Display the results.
        If ((Not result Is Nothing) AndAlso (Not result.campaignEstimates Is Nothing) _
            AndAlso (result.campaignEstimates.Length > 0)) Then
          Dim campaignEstimate As CampaignEstimate = result.campaignEstimates(0)
          If ((Not campaignEstimate.adGroupEstimates Is Nothing) AndAlso _
              (campaignEstimate.adGroupEstimates.Length > 0)) Then
            Dim adGroupEstimate As AdGroupEstimate = campaignEstimate.adGroupEstimates(0)

            If (Not adGroupEstimate.keywordEstimates Is Nothing) Then
              For i As Integer = 0 To adGroupEstimate.keywordEstimates.Length - 1
                Dim keyword As Keyword = keywordEstimateRequests.Item(i).keyword
                Dim keywordEstimate As KeywordEstimate = adGroupEstimate.keywordEstimates(i)

                If keywordEstimateRequests.Item(i).isNegative Then
                  Continue For
                End If

                ' Find the mean of the min and max values.
                Dim meanAverageCpc As Long = ((keywordEstimate.min.averageCpc.microAmount + _
                    keywordEstimate.max.averageCpc.microAmount) / 2)
                Dim meanAveragePosition As Double = ((keywordEstimate.min.averagePosition + _
                    keywordEstimate.max.averagePosition) / 2)
                Dim meanClicks As Single = ((keywordEstimate.min.clicksPerDay + _
                    keywordEstimate.max.clicksPerDay) / 2)
                Dim meanTotalCost As Long = ((keywordEstimate.min.totalCost.microAmount + _
                    keywordEstimate.max.totalCost.microAmount) / 2)

                Console.WriteLine("Results for the keyword with text = '{0}' and match type " & _
                    "= '{1}':", keyword.text, keyword.matchType)
                Console.WriteLine("  Estimated average CPC: {0}", meanAverageCpc)
                Console.WriteLine("  Estimated ad position: {0:0.00}", meanAveragePosition)
                Console.WriteLine("  Estimated daily clicks: {0}", meanClicks)
                Console.WriteLine("  Estimated daily cost: {0}", meanTotalCost)
              Next i
            End If
          End If
        Else
          Console.WriteLine("No traffic estimates were returned.\n")
        End If
      Catch ex As Exception
        Throw New System.ApplicationException("Failed to retrieve traffic estimates.", ex)
      End Try
    End Sub
  End Class
End Namespace
