$(document).ready(function () {
    $.getJSON('/Reporte/RecuperarBalanceHoras', function (data) {
        let headers = {};
        let groupedData = {};
        let totalGeneralPorMes = {};
        let totalGeneralPorAnio = {};
        let totalGeneralHoras = 0;





        data.forEach(item => {

            let mesAnioInterno = `${item.anio}-${String(item.mes).padStart(2, '0')}`;

            let mesAnioExterno = `${item.mes}-${item.anio}`;
            let anio = item.anio;

            if (!headers[mesAnioInterno]) headers[mesAnioInterno] = mesAnioExterno;
            if (!totalGeneralPorMes[mesAnioInterno]) totalGeneralPorMes[mesAnioInterno] = 0;
            if (!totalGeneralPorAnio[anio]) totalGeneralPorAnio[anio] = 0;
        });

        let sortedHeaders = Object.keys(headers).sort((a, b) => {
            const [yearA, monthA] = a.split('-').map(Number);
            const [yearB, monthB] = b.split('-').map(Number);
            return yearA === yearB ? monthA - monthB : yearA - yearB;
        });
        data.forEach(item => {
            let mesAnioInterno = `${item.anio}-${String(item.mes).padStart(2, '0')}`;
            let anio = item.anio;

            if (!groupedData[item.tiporecurso]) groupedData[item.tiporecurso] = {};

            if (!groupedData[item.tiporecurso][item.nombreproyecto]) {
                groupedData[item.tiporecurso][item.nombreproyecto] = {
                    nombreproyecto: item.nombreproyecto,
                    tipologia: item.tipologia,
                    cliente: item.nombrecliente,
                    numeroproyecto: item.numeroproyecto,
                    horas: {},
                    totalAnual: {}
                };
            }

            if (!groupedData[item.tiporecurso][item.nombreproyecto].horas[mesAnioInterno]) {
                groupedData[item.tiporecurso][item.nombreproyecto].horas[mesAnioInterno] = 0;
            }
            groupedData[item.tiporecurso][item.nombreproyecto].horas[mesAnioInterno] += item.totalhorasporrecurso;

            if (!groupedData[item.tiporecurso][item.nombreproyecto].totalAnual[anio]) {
                groupedData[item.tiporecurso][item.nombreproyecto].totalAnual[anio] = 0;
            }
            groupedData[item.tiporecurso][item.nombreproyecto].totalAnual[anio] += item.totalhorasporrecurso;

            totalGeneralPorMes[mesAnioInterno] += item.totalhorasporrecurso;
            totalGeneralPorAnio[anio] += item.totalhorasporrecurso;
            totalGeneralHoras += item.totalhorasporrecurso;
        });


        const ordenTipoRecurso = { "Socio": 1, "Staff": 2, "Consultor Externo": 3 };


        let headerRow = $('#balanceTable thead tr');
        sortedHeaders.forEach(key => {
            headerRow.append(`<th class="text-left text-xs font-medium">${headers[key]}</th>`);
        });


        Object.keys(totalGeneralPorAnio).forEach(anio => {
            headerRow.append(`<th class="text-left text-xs font-medium">Total ${anio}</th>`);
        });

        //columna final "Total General"
        headerRow.append(`<th class="text-left text-xs font-medium">Total General</th>`);


        let tbody = $('#balanceTable tbody');
        Object.keys(groupedData)
            .sort((a, b) => (ordenTipoRecurso[a] || 4) - (ordenTipoRecurso[b] || 4))
            .forEach(tiporecurso => {

                let totalHorasPorMes = {};
                let totalHorasPorAnio = {};
                let totalHorasGeneral = 0;

                Object.keys(headers).forEach(header => { totalHorasPorMes[header] = 0; });
                Object.keys(totalGeneralPorAnio).forEach(anio => { totalHorasPorAnio[anio] = 0; });

                const proyectosOrdenados = Object.values(groupedData[tiporecurso])
                    .sort((a, b) => a.tipologia.localeCompare(b.tipologia));

                proyectosOrdenados.forEach(row => {
                    let tr = `<tr>
                                        <td class='text-left text-xs font-medium '>${tiporecurso}</td>
                                        <td class='text-left text-xs font-medium '>${row.tipologia}</td>
                                        <td class='text-left text-xs font-medium '>${row.nombreproyecto}</td>
                                        <td class='text-left text-xs font-medium '>${row.cliente}</td>
                                        <td class='text-left text-xs font-medium '>${"'" + row.numeroproyecto}</td>`;

                    let totalHorasPorProyecto = 0;

                    sortedHeaders.forEach(header => {
                        let horas = row.horas[header] || 0;
                        totalHorasPorProyecto += horas;
                        totalHorasPorMes[header] += horas;
                        tr += `<td class='text-xs border font-medium text-right border-black px-4 py-2 min-w-[100px]'>${horas > 0 ? horas : ''}</td>`;
                    });

                    // totales anuales del proyecto
                    Object.keys(totalGeneralPorAnio).forEach(anio => {
                        let totalAnual = row.totalAnual[anio] || 0;
                        totalHorasPorAnio[anio] += totalAnual;
                        tr += `<td class="text-xs border font-medium text-right border-black px-4 py-2 min-w-[100px]">${totalAnual > 0 ? totalAnual : ''}</td>`;
                    });

                    // el total general del proyecto
                    tr += `<td class="text-xs border font-medium text-right border-black px-4 py-2 min-w-[100px]">${totalHorasPorProyecto > 0 ? totalHorasPorProyecto : ''}</td>`;
                    tr += '</tr>';
                    tbody.append(tr);

                    totalHorasGeneral += totalHorasPorProyecto;
                });

                // fila de total para el tiporecurso actual
                let totalRow = `<tr class="yellow">
                                <td colspan="2" class="text-left text-xs font-medium ">Total ${tiporecurso}</td>
                            <td colspan="3"></td>`;
                sortedHeaders.forEach(header => {
                    totalRow += `<td class="text-xs border font-medium text-right border-black px-4 py-2 min-w-[100px]">${totalHorasPorMes[header] > 0 ? totalHorasPorMes[header] : ''}</td>`;
                });
                Object.keys(totalGeneralPorAnio).forEach(anio => {
                    totalRow += `<td class="text-xs border font-medium text-right border-black px-4 py-2 min-w-[100px]">${totalHorasPorAnio[anio] > 0 ? totalHorasPorAnio[anio] : ''}</td>`;
                });
                totalRow += `<td class="text-xs border font-medium text-right border-black px-4 py-2 min-w-[100px]">${totalHorasGeneral > 0 ? totalHorasGeneral : ''}</td>`;
                totalRow += '</tr>';
                tbody.append(totalRow);
            });

        // fila de total general para las horas
        let totalGeneralRow = `<tr class="totales">
                        <td colspan="2"  class="text-left text-xs font-medium ">Total General</td>
                    <td colspan="3"></td>`;
        sortedHeaders.forEach(header => {
            totalGeneralRow += `<td class="text-xs border font-medium text-right border-black px-4 py-2 min-w-[100px]">${totalGeneralPorMes[header] > 0 ? totalGeneralPorMes[header] : ''}</td>`;
        });
        Object.keys(totalGeneralPorAnio).forEach(anio => {
            totalGeneralRow += `<td class="text-xs border font-medium text-right border-black px-4 py-2 min-w-[100px]">${totalGeneralPorAnio[anio] > 0 ? totalGeneralPorAnio[anio] : ''}</td>`;
        });
        totalGeneralRow += `<td class="text-xs border font-medium text-right border-black px-4 py-2 min-w-[100px]">${totalGeneralHoras > 0 ? totalGeneralHoras : ''}</td>`; // Solo sumar las horas en "Total General"
        totalGeneralRow += '</tr>';
        tbody.append(totalGeneralRow);

        /*excel*/
        $('#exportButton').on('click', async function () {
            const wb = new ExcelJS.Workbook();
            const ws = wb.addWorksheet('Balance Horas Efectivas');

            const response = await fetch('/images/unitt.png');
            const imageBlob = await response.blob();
            const arrayBuffer = await imageBlob.arrayBuffer();
            const logoImageId = wb.addImage({
                buffer: arrayBuffer,
                extension: 'png'
            });


            ws.addImage(logoImageId, {
                tl: { col: 0, row: 0 },
                ext: { width: 150, height: 50 }
            });
            ws.mergeCells('C3:E3');
            const titulo = ws.getCell('C3');
            titulo.value = "Balance Horas Efectivas";
            titulo.font = { size: 16, bold: true };
            titulo.alignment = { vertical: 'middle', horizontal: 'center' };

            ws.addRow([]);
            ws.addRow([]);

            const headerStyle = {
                font: { bold: true },
                alignment: { horizontal: 'center', vertical: 'middle' },
                border: { top: { style: 'thin' }, left: { style: 'thin' }, right: { style: 'thin' }, bottom: { style: 'thin' } },
                fill: { type: 'pattern', pattern: 'solid', fgColor: { argb: 'D6EAF8' } }
            };

            const rowStyle = {

            };


            let headers = [];
            $('#balanceTable thead th').each(function () {
                headers.push($(this).text().trim());
            });


            ws.addRow(headers);
            ws.getRow(1).eachCell(cell => {
                cell.style = headerStyle;
            });


            let rows = [];
            $('#balanceTable tbody tr').each(function () {
                let rowData = [];
                $(this).find('td').each(function () {

                    const colspan = $(this).attr('colspan');
                    if (colspan) {

                        for (let i = 0; i < colspan - 1; i++) {
                            rowData.push('');
                        }
                    }
                    rowData.push($(this).text().trim());
                });
                rows.push(rowData);
            });


            rows.forEach(row => {
                const excelRow = ws.addRow(row);
                excelRow.eachCell(cell => {

                    const cellValue = cell.text.trim();

                    if (!isNaN(cellValue) && cellValue !== '') {

                        cell.value = parseFloat(cellValue);

                        cell.alignment = { horizontal: 'right' };
                    } else {

                        cell.alignment = { horizontal: 'left' };
                    }
                    cell.style = rowStyle;
                });
            });


            ws.columns.forEach(column => {
                let maxLength = 0;
                column.eachCell({ includeEmpty: true }, function (cell) {
                    const columnLength = cell.text.length;
                    if (columnLength > maxLength) {
                        maxLength = columnLength;
                    }
                });
                column.width = maxLength < 10 ? 10 : maxLength;
            });


            wb.xlsx.writeBuffer().then(function (buffer) {
                const blob = new Blob([buffer], { type: 'application/octet-stream' });
                const link = document.createElement('a');
                link.href = URL.createObjectURL(blob);
                link.download = 'Balance_Horas_Efectivas.xlsx';
                link.click();
            });
        });

    });
});