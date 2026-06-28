$content = Get-Content 'c:\Users\Ozem\Desktop\Projeler\e-iskele Projesi\eiskele\backend\src\EIskele.Infrastructure\Services\BoatService.Admin.cs' -Raw

$content = $content -replace 'Status = f.Status.ToString\(\),', 'Status = f.Status == EIskele.Domain.Enums.StoredFileStatus.PendingReview ? "pendingReview" : f.Status == EIskele.Domain.Enums.StoredFileStatus.Approved ? "approved" : "rejected",'

$content = $content -replace 'DocumentType = f.FileType.ToString\(\),', 'DocumentType = f.FileType == EIskele.Domain.Enums.StoredFileType.BoatLicenseDocument ? "boatLicense" : "insurance",'

Set-Content -Path 'c:\Users\Ozem\Desktop\Projeler\e-iskele Projesi\eiskele\backend\src\EIskele.Infrastructure\Services\BoatService.Admin.cs' -Value $content
