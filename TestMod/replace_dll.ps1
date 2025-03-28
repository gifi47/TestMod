$source = "$(TargetDir)TestMod.dll"  # Исходный файл (после сборки)
$destination = "D:\Projects\BS\BlockStory\Mods\TestMod.dll"  # Куда копировать
$backup = "$destination.bak"  # Создание резервной копии

if (Test-Path $destination) {
    Copy-Item $destination -Destination $backup -Force
    Write-Host "Резервная копия сохранена: $backup"
}

Copy-Item $source -Destination $destination -Force
Write-Host "Файл TestMod.dll обновлён!"