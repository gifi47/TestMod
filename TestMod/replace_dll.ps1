$source = "$(TargetDir)TestMod.dll"  # Исходный файл (после сборки)
$destination = "D:\Projects\BS\BlockStory\Mods\TestMod.dll"  # Куда копировать

# Проверяем, существует ли директория назначения
$destinationDir = Split-Path -Path $destination
if (!(Test-Path $destinationDir)) {
    Write-Host "Ошибка: Директория назначения '$destinationDir' не существует. Операция отменена."
    exit 0  # Завершаем выполнение скрипта с ошибкой
}

$backup = "$destination.bak"  # Создание резервной копии

if (Test-Path $destination) {
    Copy-Item $destination -Destination $backup -Force
    Write-Host "Резервная копия сохранена: $backup"
}

Copy-Item $source -Destination $destination -Force
Write-Host "Файл TestMod.dll обновлён!"