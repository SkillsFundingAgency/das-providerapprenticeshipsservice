function RecursivelyFixVariableValue([string] $value) {
	Write-Host ('Fixing ' + $value)
	$result = $value
	
	$varRegex = '\$\(.*\)'
	$varMatches = select-string -InputObject $value -Pattern $varRegex -AllMatches | % { $_.Matches } | % { $_.Value }
	ForEach($varMatch in $varMatches)
	{
		$varKey = ($varMatch -replace '^\$\(','') -replace '\)$',''
		$varEnv = $varKey -replace '\.','_'
		$varVal = (get-item env:$varEnv).Value
		
		Write-Host ('Transform ' + $varKey + '(' + $varEnv + ') to ' + $varVal)

		$result = RecursivelyFixVariableValue($result -replace ('\$\(' + $varKey + '\)'),$varVal)
	}
	
	return $result
}


If (!$env:AutomatedBuild) {
$env:AutomatedBuild= "*.publish.xml;*.csdef;*.json;*.config;*.cscfg;web.config;app.config"
$mask = $env:AutomatedBuild.replace(' ','').split(';')
}


$mask = $env:AutomatedBuild.replace(' ','').split(';')




$SourcePath = (Get-Item -Path ".\" -Verbose).FullName

$testPath = Test-Path $SourcePath

$regex = "__[A-Za-z0-9.]*__"
$patterns = @()
$matches = @()



if($testPath)
{
	Write-Output "Path Exists"
		
	$List = Get-ChildItem $SourcePath -recurse -Include $mask
	
	
	Foreach($file in $list)
	{
		$destinationPath = $file.FullName
		$tempFile = join-path $file.DirectoryName ($file.BaseName + ".tmp")
		
		Copy-Item -Force $file.FullName $tempFile

		$matches = select-string -Path $tempFile -Pattern $regex -AllMatches | % { $_.Matches } | % { $_.Value }
		
		ForEach($match in $matches)
		{
		  Write-Output ("Attempting to match variable " + $match)
		  $matchedItem = $match
		  $matchedItem = $matchedItem.Trim('_')
		  $matchedItem = $matchedItem -replace '\.','_'
		  
		  $newValue = RecursivelyFixVariableValue((get-item env:$matchedItem).Value)
		  
		  Write-Output ("Replacing " + $match + " with " + $newValue)
		  
		  (Get-Content $tempFile) | 
		  Foreach-Object {
			$_ -replace $match,$newValue
		  } | 
		Set-Content $tempFile -Force
		}

		Copy-Item -Force $tempFile $DestinationPath
		Remove-Item -Force $tempFile
	}
}
else
{
	Write-Output "Path Does Not Exist"
}