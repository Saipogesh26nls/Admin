$(document).ready(function () {

    getdata();
    function getdata() {
        $.ajax({
            url: 'http://localhost:3000/user/',
            method: 'get',
            dataType: 'json',
            success: function (response) {
                $("#nodejs_test").find("tr:not(:first)").remove();
                for (i = 0; i <= response.data.length - 1; i++) {
                    $('#nodejs_test tbody').append("<tr class='taskrow'><td>" + response.data[i].name + "</td><td>" + response.data[i].age + "</td><td>" + response.data[i].address + "</td><td>" + response.data[i].salary + "</td></tr>");
                }
            },
            error: function (response) {
                alert('server error');
            }
        });
    }

    $('#submit').click(function () {
        var name = $("#name").val();
        var age = parseInt($("#age").val());
        var address = $("#address").val();
        var salary = parseInt($("#salary").val());
        $.ajax({
            url: 'http://localhost:3000/user',
            method: 'post',
            dataType: 'json',
            data: { "name": name, "age": age, "address": address, "salary": salary },
            success: function (response) {
                if (response.message == 'Data inserted') {
                    alert('Data added successfully');
                    getdata();
                    $('#name').val('');
                    $('#age').val('');
                    $('#address').val('');
                    $('#salary').val('');
                } else {
                    alert('some error occurred try again');
                }
            },
            error: function (response) {
                alert('server error occured');
            }
        });
    });
});
