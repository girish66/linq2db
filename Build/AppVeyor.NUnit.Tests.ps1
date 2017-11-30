$wc = New-Object System.Net.WebClient
#$wc.Timeout = 180000

$logFileName = "$env:APPVEYOR_BUILD_FOLDER\nunit_net452_results.xml"

nunit3-console Tests\Linq\bin\AppVeyor\net452\linq2db.Tests.dll --result=$logFileName --where "cat != Ignored"

if ($LastExitCode -ne 0) { $host.SetShouldExit($LastExitCode) }
echo "UploadFile: https://ci.appveyor.com/api/testresults/nunit3/$env:APPVEYOR_JOB_ID from $logFileName"
$wc.UploadFile("https://ci.appveyor.com/api/testresults/nunit3/$env:APPVEYOR_JOB_ID", "$logFileName")
if ($LastExitCode -ne 0) {
	echo "FAIL: UploadFile: https://ci.appveyor.com/api/testresults/nunit3/$env:APPVEYOR_JOB_ID from $logFileName"
	$host.SetShouldExit($LastExitCode)
}


$logFileName = "$env:APPVEYOR_BUILD_FOLDER\nunit_core2_results.xml"

dotnet test Tests\Linq\ -f netcoreapp2.0 --logger:"trx;LogFileName=$logFileName" --filter:"TestCategory!=Ignored" -c AppVeyor

if ($LastExitCode -ne 0) {
	echo "FAIL: dotnet vstest $a --logger:'trx;LogFileName=$logFileName'"
	$host.SetShouldExit($LastExitCode)
}

echo "UploadFile: https://ci.appveyor.com/api/testresults/nunit3/$env:APPVEYOR_JOB_ID from $logFileName"
$wc.UploadFile("https://ci.appveyor.com/api/testresults/mstest/$env:APPVEYOR_JOB_ID", "$logFileName")
if ($LastExitCode -ne 0) {
	echo "FAIL: UploadFile: https://ci.appveyor.com/api/testresults/mstest/$env:APPVEYOR_JOB_ID from $logFileName"
	$host.SetShouldExit($LastExitCode)
}


$logFileName = "$env:APPVEYOR_BUILD_FOLDER\nunit_core1_results.xml"

dotnet test Tests\Linq\ -f netcoreapp1.0 --logger:"trx;LogFileName=$logFileName" --filter:"TestCategory!=Ignored" -c AppVeyor

if ($LastExitCode -ne 0) {
	echo "FAIL: dotnet vstest $a --logger:'trx;LogFileName=$logFileName'"
	$host.SetShouldExit($LastExitCode)
}

echo "UploadFile: https://ci.appveyor.com/api/testresults/nunit3/$env:APPVEYOR_JOB_ID from $logFileName"
$wc.UploadFile("https://ci.appveyor.com/api/testresults/mstest/$env:APPVEYOR_JOB_ID", "$logFileName")
if ($LastExitCode -ne 0) {
	echo "FAIL: UploadFile: https://ci.appveyor.com/api/testresults/mstest/$env:APPVEYOR_JOB_ID from $logFileName"
	$host.SetShouldExit($LastExitCode)
}
