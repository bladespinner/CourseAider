$(document).ready(function () {
    $("#btnUpload").click(OnUpload);
    $(".fileRate").click(function (e) {
        $this = $(this);
        $.ajax({
            type: "POST",
            url: "/api/FileRating/" + $(this).attr("data-file-id"),
            success: function () {
                $this.next().show();
                $this.hide();
            }
        })
    });
});

function ShowUploadControls() {
    $("#uploadControls").show();
    $("#uploadProgress").hide();
}
function ShowUploadProgress() {
    $("#uploadControls").hide();
    $("#uploadProgress").show();
}

function OnUpload(evt) {
    var files = $("#fileUpload").get(0).files;
    if (files.length > 0) {

        ShowUploadProgress();

        if (window.FormData !== undefined) {
            var data = new FormData();
            for (i = 0; i < files.length; i++) {
                data.append("file" + i, files[i]);
            }
            data.append("visible", $("#fileVisibility").prop('checked'))
            $.ajax({
                type: "POST",
                url: uploadPath,
                contentType: false,
                processData: false,
                data: data,
                success: function (results) {
                    ShowUploadControls();
                    var visiblity = $("#fileVisibility").prop('checked');
                    for (i = 0; i < results.length; i++) {
                        var html = "<tr>";
                        var splited = results[i].split('/');
                        html += "<td>" + htmlEncode(splited[splited.length - 1]) + "</td>";
                        html += "<td><i>You</i></td>";
                        if (visiblity)
                        {
                            html += "<td>Public</td>";
                        }
                        else
                        {
                            html += "<td>Private</td>";
                        }
                        html += "<td><a href=" + htmlEncode(results[i]) + ">Download</a></td>";
                        html += "</tr>";
                        $("#files-list").append(html);
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    ShowUploadControls();
                    alert(xhr.responseText);
                }
            });
        } else {
            alert("Your browser doesn't support HTML5 multiple file uploads! Please use some decent browser.");
        }
    }
}