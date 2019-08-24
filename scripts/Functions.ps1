function Assert-Cmd-Ok($alsoAcceptable) {
    if ($LASTEXITCODE -ne 0 -and $LASTEXITCODE -ne $alsoAcceptable) {
        Write-Error "Build not succeeded... See errors"
        Exit -1
    }
}