param(
    [Parameter(Mandatory=$True, Position=0, ValueFromPipeline=$false)]
    [decimal]
    $threshold = 80
)

ReportGenerator.exe "-reports:opencover.xml" "-targetdir:./coverage" "-reporttypes:textSummary"
$fileContent = Get-Content -Path ./coverage/Summary.txt -TotalCount 11
$covStr = [regex]::match($fileContent,'Line coverage: (\d+\.?\d?)%').Groups[1].Value
$cov = [decimal]$covStr
if($cov -lt $threshold) {throw "coverage is not meeting expectations. Expected: $threshold, Actual: $cov"}