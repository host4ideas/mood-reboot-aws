//  tag, create shortcut to access PDF.js exports.
var pdfjsLib = window['pdfjs-dist/build/pdf'];
// The workerSrc property shall be specified.
pdfjsLib.GlobalWorkerOptions.workerSrc = './js/pdfjs-3.3.122-dist/build/pdf.worker.js';

$("#hidden-file").on("change", function (e) {
    var file = e.target.files[0];

    // Clean previews
    $('#imgImagePreview').empty();
    $('#canvasPdfPreview').empty();
    $("#tableDataPreview").empty();
    // Show preview controls
    $('#toggleExcelPreview').removeClass("hidden");
    $('#toggleImgPreview').removeClass("hidden");
    $('#togglePdfPreview').removeClass("hidden");

    if (file.type == "application/pdf") {
        try {
            var fileReader = new FileReader();
            fileReader.onload = function () {
                var pdfData = new Uint8Array(this.result);
                // Using DocumentInitParameters object to load binary data.

                var pdfDoc = null,
                    pageNum = 1,
                    pageRendering = false,
                    pageNumPending = null,
                    scale = 0.8,
                    canvas = document.getElementById('canvasPdfPreview'),
                    ctx = canvas.getContext('2d');

                /**
                 * Get page info from document, resize canvas accordingly, and render page.
                 * @param num Page number.
                 */
                function renderPage(num) {
                    pageRendering = true;
                    // Using promise to fetch the page
                    pdfDoc.getPage(num).then(function (page) {
                        var viewport = page.getViewport({ scale: scale });
                        canvas.height = viewport.height;
                        canvas.width = viewport.width;

                        // Render PDF page into canvas context
                        var renderContext = {
                            canvasContext: ctx,
                            viewport: viewport
                        };
                        var renderTask = page.render(renderContext);

                        // Wait for rendering to finish
                        renderTask.promise.then(function () {
                            pageRendering = false;
                            if (pageNumPending !== null) {
                                // New page rendering is pending
                                renderPage(pageNumPending);
                                pageNumPending = null;
                            }
                        });
                    });

                    // Update page counters
                    document.getElementById('page_num').textContent = num;
                    $("#pageProgressBar").css("width", Math.round((num * 100) / pdfDoc.numPages) + "%");
                }

                /**
                 * If another page rendering in progress, waits until the rendering is
                 * finised. Otherwise, executes rendering immediately.
                 */
                function queueRenderPage(num) {
                    if (pageRendering) {
                        pageNumPending = num;
                    } else {
                        renderPage(num);
                    }
                }

                /**
                 * Displays previous page.
                 */
                function onPrevPage() {
                    if (pageNum <= 1) {
                        return;
                    }
                    pageNum--;
                    queueRenderPage(pageNum);
                }
                document.getElementById('prev').addEventListener('click', onPrevPage);

                /**
                 * Displays next page.
                 */
                function onNextPage() {
                    if (pageNum >= pdfDoc.numPages) {
                        return;
                    }
                    pageNum++;
                    queueRenderPage(pageNum);
                }
                document.getElementById('next').addEventListener('click', onNextPage);

                /**
                 * Asynchronously downloads PDF.
                 */
                pdfjsLib.getDocument({ data: pdfData }).promise.then(function (pdfDoc_) {
                    pdfDoc = pdfDoc_;
                    ; $("#page_count").text(pdfDoc.numPages);
                    $("#previewContent").removeClass("hidden");

                    // Initial/first page rendering
                    renderPage(pageNum);
                });
            };
            fileReader.readAsArrayBuffer(file);
        } catch (e) {
            console.error(e.message);
            $("#previewContent").addClass("hidden");
        }
    }

    if (file.type == "image/jpeg" || file.type == "image/png" || file.type == "image/webp" || file.type == "image/gif") {
        var reader = new FileReader();
        reader.onload = function () {
            $('#imgImagePreview').attr("src", reader.result);
            $("#previewImage").removeClass("hidden");
        };
        reader.readAsDataURL(e.target.files[0]);
    }

    if (file.type == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" || file.type == "application/vnd.ms-excel") {
        var reader = new FileReader();
        reader.readAsArrayBuffer(e.target.files[0]);
        reader.onload = function (e) {

            var data = new Uint8Array(reader.result);
            var work_book = XLSX.read(data, { type: 'array' });
            var sheet_name = work_book.SheetNames;
            var sheet_data = XLSX.utils.sheet_to_json(work_book.Sheets[sheet_name[0]], { header: 1 });

            if (sheet_data.length > 0) {
                var table_output = '<table class="w-full text-sm text-left text-gray-500 dark:text-gray-400">';

                for (var row = 0; row < sheet_data.length; row++) {

                    table_output += '<tr class="bg-white border-b dark:bg-gray-800 dark:border-gray-700">';

                    for (var cell = 0; cell < sheet_data[row].length; cell++) {

                        if (row == 0) {
                            table_output += '<th scope="row" class="px-6 py-4 font-medium text-gray-900 whitespace-nowrap dark:text-white">' + sheet_data[row][cell] + '</th>';
                        }
                        else {
                            table_output += '<td class="px-6 py-4">' + sheet_data[row][cell] + '</td>';
                        }
                    }
                    table_output += '</tr>';
                }
                table_output += '</table>';
                $("#tableDataPreview").removeClass("hidden").html(table_output);
            }
        }
    }
});

$("#toggleImgPreview").click(function () {
    $("#imgImagePreview").toggleClass("hidden");
});

$("#toggleExcelPreview").click(function () {
    $("#tableDataPreview").toggleClass("hidden");
});

$("#togglePdfPreview").click(function () {
    $("#previewContent").toggleClass("hidden");
});