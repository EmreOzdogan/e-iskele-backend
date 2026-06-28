$content = Get-Content 'c:\Users\Ozem\Desktop\Projeler\e-iskele Projesi\eiskele\backend\src\EIskele.Infrastructure\Services\BoatService.Admin.cs' -Raw

$content = $content -replace 'f.FileType == EIskele.Domain.Enums.StoredFileType.BoatLicenseDocument \?', 'f.FileType == EIskele.Domain.Enums.StoredFileType.BoatLicenseDocument.ToString() ?'

Set-Content -Path 'c:\Users\Ozem\Desktop\Projeler\e-iskele Projesi\eiskele\backend\src\EIskele.Infrastructure\Services\BoatService.Admin.cs' -Value $content
