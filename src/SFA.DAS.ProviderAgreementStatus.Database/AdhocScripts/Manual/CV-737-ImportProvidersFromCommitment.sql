/*
Script to populate the deleted Provider data for Approvals from Commitment to PAS

Instructions for use:
1. Turn on SQL CMD mode and Results to Text and max characters in text output to 8192 (query->query options->results->text->Maximum number of characters displayed in each column)
2. Execute this script against Commitment database
3. Execute the resulting script against PAS database
*/

  Select 'IF NOT EXISTS(SELECT 1 FROM Providers WHERE Ukprn = ' + '''' +  convert(varchar,[Ukprn]) + '''' + ')' + char(13) + char(10) 
  + 'Insert into Providers ([Ukprn], [Name], [Created]) values' + char(13) + char(10)
  + '( ' + '''' +  convert(varchar,[Ukprn]) + ''''  + ', ' + '''' + convert(varchar,[Name]) + '''' +', ' + '''' + convert(NVARCHAR(MAX),[Created],121) + '''' + ');'  + char(13) + char(10) 
  + char(13) + char(10)
  From [dbo].[Providers]