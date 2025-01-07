const nombreProyecto = document.getElementById('proyectoData').dataset.nombreproyecto;

async function exportarTablaExcel(tablaId, nombreArchivo) {

    const workbook = new ExcelJS.Workbook();
    const worksheet = workbook.addWorksheet('Flujo de Caja');
    worksheet.views = [{ state: 'frozen', xSplit: 4, ySplit: 6 }];

    const response = await fetch('/images/unitt.png');
    const imageBlob = await response.blob();
    const arrayBuffer = await imageBlob.arrayBuffer();
    const logoImageId = workbook.addImage({
        buffer: arrayBuffer,
        extension: 'png',
    });


    worksheet.addImage(logoImageId, {
        tl: { col: 0, row: 0 },
        ext: { width: 150, height: 50 },
    });


    worksheet.mergeCells('B3:D3');
    const titleCell = worksheet.getCell('B3');
    titleCell.value = `Flujo de Caja Proyecto: ${nombreProyecto}`;
    titleCell.font = { bold: true, size: 14 };
    titleCell.alignment = { horizontal: 'center', vertical: 'middle' };


    const startRow = 5;


    const tabla = document.getElementById(tablaId);


    const rows = tabla.rows;
    for (let i = 0; i < rows.length; i++) {
        const row = rows[i];
        const excelRow = worksheet.getRow(startRow + i);

        let colIndex = 1;
        for (let j = 0; j < row.cells.length; j++) {
            const cell = row.cells[j];
            const excelCell = excelRow.getCell(colIndex);
            const rawHtml = cell.innerHTML.trim();
            let rawText = rawHtml.replace(/<[^>]*>/g, '').trim()
            const cleanedText = rawText.replace(/\./g, '').replace(',', '.').replace(/\+/g, '').trim();

            if (rawText.includes('%')) {
                const cleanedTextWithoutPercent = cleanedText.replace('%', '').trim();
                if (!isNaN(cleanedTextWithoutPercent) && cleanedTextWithoutPercent !== '') {
                    const porcentajeValue = parseFloat(cleanedTextWithoutPercent);
                    excelCell.value = porcentajeValue / 10000;
                    excelCell.numFmt = '0.00%';
                    excelCell.alignment = { vertical: 'middle', horizontal: 'right' };
                } else {
                    excelCell.value = rawText;

                    if (rawText.includes('%') && cleanedTextWithoutPercent !== '') {
                        excelCell.alignment = { vertical: 'middle', horizontal: 'left' };
                    } else {
                        excelCell.alignment = { vertical: 'middle', horizontal: 'right' };
                    }
                }
            } else if (!isNaN(cleanedText) && cleanedText !== '') {
                const cellValue = parseFloat(cleanedText);
                excelCell.value = cellValue;
                excelCell.numFmt = '#,##0';
                excelCell.alignment = { vertical: 'middle', horizontal: 'right' };
            } else {
                if (rawText.includes('+')) {
                    rawText = '';
                }
                excelCell.value = rawText;
                excelCell.alignment = { vertical: 'middle', horizontal: 'left' };
            }


            if (cell.colSpan > 1) {
                worksheet.mergeCells(startRow + i, colIndex, startRow + i, colIndex + cell.colSpan - 1);
            }

            const computedStyle = window.getComputedStyle(cell);
            const textColor = computedStyle.color || '#000000';
            excelCell.font = { color: { argb: rgbToHex(textColor) }, bold: i === 0 || row.classList.contains('bg-gray-200') };

            excelCell.border = {
                top: { style: 'thin' },
                left: { style: 'thin' },
                bottom: { style: 'thin' },
                right: { style: 'thin' },
            };

            colIndex += cell.colSpan || 1;
        }
    }

    worksheet.columns.forEach(column => {
        column.width = 20;
    });

    const buffer = await workbook.xlsx.writeBuffer();
    const blob = new Blob([buffer], { type: 'application/octet-stream' });
    const link = document.createElement('a');
    link.href = URL.createObjectURL(blob);
    link.download = `${nombreArchivo}_${nombreProyecto}.xlsx`;
    link.click();
}

function rgbToHex(rgb) {
    const match = rgb.match(/\d+/g);
    if (!match) return 'FFFFFF';
    const [r, g, b] = match.map(Number);
    return `${toHex(r)}${toHex(g)}${toHex(b)}`;
}

function toHex(c) {
    const hex = c.toString(16);
    return hex.length === 1 ? '0' + hex : hex;
}

document.getElementById('btnExportarExcel').addEventListener('click', function () {
    exportarTablaExcel('TablaFlujoCaja', 'FlujoCaja');
});