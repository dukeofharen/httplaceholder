function Assert-Cmd-Ok
{
    if($LASTEXITCODE -ne 0)
    {
        Write-Error "Build not succeeded... See errors"
        Exit -1
    }
}