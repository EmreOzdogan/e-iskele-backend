$content = Get-Content 'c:\Users\Ozem\Desktop\Projeler\e-iskele Projesi\eiskele\backend\src\EIskele.Infrastructure\Services\BoatService.Admin.cs' -Raw

$content = $content -replace 'StoredFileStatus\.PendingReview', 'StoredFileStatus.Pending'

$content = $content -replace 'ProductionYear = boat\.ProductionYear,', 'ProductionYear = int.TryParse(boat.ProductionYear, out var pYear) ? pYear : null,'
$content = $content -replace 'Length = boat\.Length,', 'Length = decimal.TryParse(boat.Length, out var bLen) ? bLen : null,'

Set-Content -Path 'c:\Users\Ozem\Desktop\Projeler\e-iskele Projesi\eiskele\backend\src\EIskele.Infrastructure\Services\BoatService.Admin.cs' -Value $content
