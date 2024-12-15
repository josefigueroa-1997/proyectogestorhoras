
/*FORMATEAR NUMEROS*/

document.addEventListener('DOMContentLoaded', function () {

    function parseNumber(value) {
        return parseFloat(value.replace(/\./g, '').replace(',', '.')) || 0;
    }

    function formatNumber(value) {
        return value.toLocaleString('de-DE', { minimumFractionDigits: 0, maximumFractionDigits: 0 });
    }

    /* function calculateMontoCLP(fila) {
         const montousdInput = fila.querySelector('input[name*="Montous"]');
         const tcInput = fila.querySelector('input[name*="Tc"]');
         const montoclpInput = fila.querySelector('input[name*="Montoclp"]');

         if (montousdInput && tcInput && montoclpInput) {
             const montousd = parseNumber(montousdInput.value);
             const tc = parseNumber(tcInput.value);
             const montoclp = montousd * tc;

             montoclpInput.value = formatNumber(montoclp);
             montoclpInput.setAttribute('readonly', true);
         }
     }

     function aplicarcambioMoneda() {
         const moneda = document.querySelector('td[data-moneda]')?.getAttribute('data-moneda');
         if (moneda === "CLP") {
             document.querySelectorAll('input[name*="Montous"], input[name*="Tc"]').forEach(input => {
                 input.value = '0';
                 input.setAttribute('readonly', true);
             });
             document.querySelectorAll('input[name*="Montoclp"]').forEach(input => {
                 input.removeAttribute('readonly');
             });
         } else {
             const tabla = document.getElementById('tablaingresos');
             tabla.addEventListener('input', function (event) {
                 const target = event.target;
                 if (target.matches('input[name*="Montous"], input[name*="Tc"]')) {
                     const fila = target.closest('tr');
                     calculateMontoCLP(fila);
                 }
             });
             document.querySelectorAll('input[name*="Montoclp"]').forEach(input => {

                 input.setAttribute('readonly', true);
             });
         }
     }

     aplicarcambioMoneda();*/


    function formatCurrency(event) {
        let value = event.target.value;


        value = value.replace(/[^\d-]/g, '');


        if (value.indexOf('-') > 0) {
            value = value.replace(/-/g, '');
        }


        if (value !== '' && value !== '-') {

            const isNegative = value.startsWith('-');
            value = value.replace(/-/g, '');


            let formattedValue = value.replace(/\B(?=(\d{3})+(?!\d))/g, '.');


            if (isNegative) {
                formattedValue = '-' + formattedValue;
            }

            event.target.value = formattedValue;
        } else {

            event.target.value = value;
        }
    }

    function removePoints(event) {
        let value = event.target.value;


        value = value.replace(/\./g, '');
        event.target.value = value;
    }


    function cleanInputsOnSubmit(event) {

        const inputs = event.target.querySelectorAll('input[name*="Montous"], input[name*="Montoclp"], input[name*="Iva"]');

        inputs.forEach(input => {

            let value = input.value;
            value = value.replace(/\./g, '');
            input.value = value;
        });
    }


    const montoInputs = document.querySelectorAll('input[name*="Montous"], input[name*="Montoclp"], input[name*="Iva"]');


    montoInputs.forEach(input => {
        input.addEventListener('input', formatCurrency);
    });


    const form = document.querySelector('form');
    if (form) {
        form.addEventListener('submit', cleanInputsOnSubmit);
    }

    document.getElementById("agregarFilaIngresos").addEventListener("click", function () {

        setTimeout(() => {

            const nuevosMontos = document.querySelectorAll('input[name*="Montoclp"], input[name*="Iva"]');

            nuevosMontos.forEach(input => {
                if (!input.hasAttribute('data-format-listener')) {
                    input.addEventListener('input', formatCurrency);
                    input.setAttribute('data-format-listener', 'true');
                }
            });
        }, 100);
    });

    /*AGREGAR NUEVA FILA*/
    $(document).ready(function () {
        $('#agregarFilaIngresos').on('click', function (e) {
            e.preventDefault();

            const nuevaFila = `
                    <tr>
                        <td class="p-2 border border-gray-300 text-left"><input type="text" name="Numdocumento" class="form-control text-right px-3 py-2 border border-gray-300 rounded focus:outline-none focus:ring-2 focus:ring-blue-500 w-20" /></td>
                        <td class="p-2 border border-gray-300 text-left"><input type="date" name="FechaEmision" class="form-control text-right px-3 py-2 border border-gray-300 rounded focus:outline-none focus:ring-2 focus:ring-blue-500 w-30" /></td>
                        <td class="p-2 border border-gray-300 text-left"><input type="date" name="FechaPago" class="form-control text-right px-3 py-2 border border-gray-300 rounded focus:outline-none focus:ring-2 focus:ring-blue-500 w-30" /></td>
                        
                        <td class="p-2 border border-gray-300 text-right"><input type="text" name="Montoclp" class="form-control text-right px-3 py-2 border border-gray-300 rounded focus:outline-none focus:ring-2 focus:ring-blue-500 w-25" readonly /></td>
                        <td class="p-2 border border-gray-300 text-right"><input type="text" name="Iva" class="form-control text-right px-3 py-2 border border-gray-300 rounded focus:outline-none focus:ring-2 focus:ring-blue-500 w-25" /></td>
                        <td class="p-2 border border-gray-300 text-left">
                                    <select name="Estado" class="px-4 py-2 text-sm border border-gray-300 rounded focus:outline-none focus:ring-2 focus:ring-blue-500 ">
                                        <option value="Forecast" selected>Forecast</option>
                                        <option value="Pagada">Pagada</option>
                                    </select>
                        </td>
                        <td class="p-2 border border-gray-300 text-right"><input name="Idcuenta" class="form-control text-right px-3 py-2 border border-gray-300 rounded focus:outline-none focus:ring-2 focus:ring-blue-500 w-20" /></td>
                        <td class="p-2 border border-gray-300 text-left"><textarea rows="2" name="Observacion" class="form-control text-left px-3 py-2 border border-gray-300 rounded focus:outline-none focus:ring-2 focus:ring-blue-500 w-48"></textarea></td>
                        <td style="display:none;">
                                    <input type="hidden" name="IdIngresoreal" value="" />
                        </td>
                    </tr>
                `;

            $('#tablaingresos tbody').append(nuevaFila);
            //aplicarcambioMoneda();
        });
    });

});