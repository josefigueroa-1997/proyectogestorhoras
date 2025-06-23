async function exportarExcelEERR(nombre) {
    const table = document.getElementById("tablaestadoresultado");
    if (!table) return alert("No se encontró la tabla.");

    const workbook = new ExcelJS.Workbook();
    const worksheet = workbook.addWorksheet("Estado de Resultados");

    worksheet.views = [{ state: 'frozen', xSplit: 1, ySplit: 6 }];

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
    titleCell.value = `Estado Resultado ${nombre}`;
    titleCell.font = { bold: true, size: 14 };
    titleCell.alignment = { horizontal: 'center', vertical: 'middle' };


    const startRow = 5;

    const rows = Array.from(table.rows);
    rows.forEach((htmlRow, rowIndex) => {
        const cells = Array.from(htmlRow.cells);
        const rowData = [];
        cells.forEach((cell, colIndex) => {
            const text = cell.textContent.trim();

            const alignRight = cell.classList.contains("text-right");
            const isCurrency = /^\$/.test(text);
            const isPercentage = text.endsWith("%");


            let value = text;
            if (isCurrency) {
                value = parseFloat(text.replace(/\$/g, '').replace(/\./g, '').replace(/,/g, '.')) || '';
            } else if (isPercentage) {
                value = parseFloat(text.replace('%', '')) / 100 || '';
            }


            rowData.push({
                value: value === 0 ? '' : value,
                alignment: { horizontal: alignRight || isCurrency || isPercentage ? 'right' : 'left' },
                numFmt: isCurrency ? '"$"#,##0' : (isPercentage ? '0.00%' : undefined)
            });
        });
        worksheet.insertRow(startRow + rowIndex, rowData.map(c => c.value));



        rowData.forEach((c, i) => {
            const cell = worksheet.getRow(startRow + rowIndex).getCell(i + 1);

            cell.alignment = c.alignment;
            if (c.numFmt) cell.numFmt = c.numFmt;
            if (rowIndex === 0) {
                cell.font = { bold: true };
                cell.fill = {
                    type: 'pattern',
                    pattern: 'solid',
                    fgColor: { argb: 'FFCCE5FF' }
                };
            }
            const tiposEnNegrita = [
                "Ingresos de la Explotación",
                "Costos Directos",
                "Gastos de Administración y Ventas",
                "Ingresos fuera de la Explotación",
                "Inversion",
                "Egresos fuera de la Explotación",
                "Intereses, Impuestos, Depreciaciones y Amortizaciones",
                "Utilidad neta",
                "EBITDA",
                "Margen de contribución",
                "% Margen de contribución",
                "% Utilidad neta",
                
            ];


            if (i === 0 && tiposEnNegrita.includes(c.value)) {
                worksheet.getRow(startRow + rowIndex).eachCell((cell) => {
                    cell.fill = {
                        type: 'pattern',
                        pattern: 'solid',
                        fgColor: { argb: 'FFD9D9D9' }
                    };
                    cell.font = { bold: true };
                });
            }
        });

    });
    worksheet.columns.forEach(column => {
        column.width = 20;
    });

    const blob = await workbook.xlsx.writeBuffer();
    const blobURL = URL.createObjectURL(new Blob([blob], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" }));
    const a = document.createElement("a");
    a.href = blobURL;
    a.download = `EstadoResultados ${nombre}.xlsx`;

    document.body.appendChild(a);
    a.click();
    a.remove();
}