var qty = 0;
var total = 0;
var final_total = 0;
var i = 1;
var val = "";
$('#Insert').click(function () {
    var _PODate = $('#Purchase_Order_Date').val();
    var _RefNo = $('#Ref_No').val();
    var _RefDate = $('#Ref_Date').val();
    var _supplier = $('#Supplier').val();
    var _billto = $('#BillTo').val();
    var _project = $('#Project').val();
    var _partno = $('#Part_No').val();
    var _qty = parseInt(document.getElementById("Quantity").value);
    var _price = $('#Purchase_Rate').val();
    var _discount_per = $('#Discount').val();
    var _igst_per = $('#IGST').val();
    var _cgst_per = $('#CGST').val();
    var _sgst_per = $('#SGST').val();

    var last_row1 = document.getElementById("selected_units").tBodies[0].rows.length;
    var l_row1 = last_row1 - 1;
    var count = 1;
    for (var i = 0; i < l_row1; i++) {
        var e_part_no = document.getElementById("selected_units").tBodies[0].rows[count].cells[0].innerHTML;
        var e_partno = document.getElementById("Part_No").value;
        if (e_part_no == e_partno) {
            var qty_err = '<p style="color:red;">Part No is already exists !!!<p/>';
            document.getElementById("value").innerHTML = qty_err;
            return;
        }
        count++;
    }
    var _subtotal = _qty * _price;
    var _discount_val;
    var _igst_val;
    var _cgst_val;
    var _sgst_val;
    switch (true) {
        case _discount_per == "":
            _discount_val = 0;
            _discount_per = 0;
            break;
        default:
            _discount_val = (_subtotal * _discount_per) / 100;
    }
    switch (true) {
        case _igst_per == "":
            _igst_val = 0;
            _igst_per = 0;
            break;
        default:
            _igst_val = (_subtotal * _igst_per) / 100;
    }
    switch (true) {
        case _cgst_per == "":
            _cgst_val = 0;
            _cgst_per = 0;
            break;
        default:
            _cgst_val = (_subtotal * _cgst_per) / 100;
    }
    switch (true) {
        case _sgst_per == "":
            _sgst_val = 0;
            _sgst_per = 0;
            break;
        default:
            _sgst_val = (_subtotal * _sgst_per) / 100;
    }

    var _total = (_subtotal - _discount_val) + _igst_val + _cgst_val + _sgst_val;

    if (_partno == "" || _qty == "" || _price == "") {
        document.getElementById("final_Quantity").value = 0;
        document.getElementById("final_SubTotal").value = "0.00";
    }
    else {
        qty = qty + _qty;
        var Total = _total;
        total = total + Total;
        document.getElementById("final_Quantity").value = qty;
        document.getElementById("final_SubTotal").value = total.toFixed(2);
    }
    var model = new Object();
    model.Part_No = document.getElementById("Part_No").value;

    jQuery.ajax({
        type: "POST",
        url: "@Url.Action('P_to_DQ')",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({ name: model }),
        success: function (data) {
            if (_partno == "") {
                var error = '<p style="color:red;">Enter Part No !!!<p/>';
                document.getElementById("value").innerHTML = error;
                return;
            }
            else if (_qty == "" || _qty == 0) {
                var error = '<p style="color:red;">Enter Quantity !!!<p/>';
                document.getElementById("value").innerHTML = error;
                return;
            }
            else {
                document.getElementById("value").innerHTML = val;
                var newrow = '<tr id="data" style="font-size:13px;"><td>' + _partno + '</td><td>' + data[0].Description + '</td><td id="Qty' + i + '">' + _qty + '</td><td>' + _price + '</td><td>' + _discount_per + '</td><td>' + _discount_val.toFixed(2) + '</td><td>' + _igst_per + '</td><td>' + _igst_val.toFixed(2) + '</td><td>' + _cgst_per + '</td><td>' + _cgst_val.toFixed(2) + '</td><td>' + _sgst_per + '</td><td>' + _sgst_val.toFixed(2) + '</td><td>' + _subtotal.toFixed(2) + '</td><td id="total' + i + '">' + _total.toFixed(2) + '</td><td style="display:none;">' + _RefNo + '</td><td style="display:none;">' + _RefDate + '</td><td style="display:none;" id="_final_qty"></td><td style="display:none;" id="_final_subtotal"></td><td style="display:none;" id="_final_total"></td><td style="display:none;" id="_final_dis_per"></td><td style="display:none;" id="_final_dis_val"></td><td style="display:none;" id="_final_igst_per"></td><td style="display:none;" id="_final_igst_val"></td><td style="display:none;" id="_final_cgst_per"></td><td style="display:none;" id="_final_cgst_val"></td><td style="display:none;" id="_final_sgst_per"></td><td style="display:none;" id="_final_sgst_val"></td><td style="display:none;">' + _supplier + '</td><td style="display:none;">' + _project + '</td><td style="display:none;">' + _PODate + '</td><td style="display:none;">' + _billto + '</td></tr>';
                $('#selected_units tr:last').after(newrow);
                formClear();
                i++;
            }
        },
        failure: function (errMsg) {
            alert(errMsg);
        }
    });
});

function vno() {
    document.getElementById("Purchase_Order_No").value = "";
}

function formClear() {
    $("#Part_No").val("");
    $("#Quantity").val("");
    $("#Purchase_Rate").val("");
    $("#Discount").val("");
    $("#IGST").val("");
    $("#CGST").val("");
    $("#SGST").val("");
}

function final_calculation() {
    var sub_total = document.getElementById("total_display").innerHTML;
    var final_discount = $("#Final_Discount").val();
    var final_tax1 = $("#Final_Tax1").val();
    var final_tax2 = $("#Final_Tax2").val();
    if (final_discount == "") {
        var discount_val = 0;
    }
    else {
        var discount_val = (sub_total * final_discount) / 100;
    }
    if (final_tax1 == "") {
        var tax1_val = 0;
    }
    else {
        var tax1_val = (sub_total * final_tax1) / 100;
        document.getElementById("total_tax").innerHTML = tax1_val.toFixed(2);
    }
    if (final_tax2 == "") {
        var tax2_val = 0;
    }
    else {
        var tax2_val = (sub_total * final_tax2) / 100;
    }
    var Total = (sub_total - discount_val) + tax1_val + tax2_val;
    document.getElementById("Final_Total").innerHTML = Total.toFixed(2);
}

$('#Submit').click(function (e) {
    var part = document.getElementById("selected_units").tBodies[0].rows.length;
    if (part == 1) {
        document.getElementById("Enter_Inv_No").innerHTML = "Table is Empty !!!";
        return;
    }
    else {
        var __final_qty = document.getElementById("final_Quantity").value;
        var __final_subtotal = document.getElementById("final_SubTotal").value;
        var __final_dis_per = document.getElementById("Final_Discount").value;
        var __final_dis_val = document.getElementById("total_discount").value;
        var __final_igst_per = document.getElementById("Total_IGST").value;
        var __final_igst_val = document.getElementById("total_igst").value;
        var __final_cgst_per = document.getElementById("Total_CGST").value;
        var __final_cgst_val = document.getElementById("total_cgst").value;
        var __final_sgst_per = document.getElementById("Total_SGST").value;
        var __final_sgst_val = document.getElementById("total_sgst").value;
        var __final_total = document.getElementById("Final_Total").value;
        document.getElementById("_final_qty").innerHTML = __final_qty;
        document.getElementById("_final_subtotal").innerHTML = __final_subtotal;
        document.getElementById("_final_dis_per").innerHTML = __final_dis_per;
        document.getElementById("_final_dis_val").innerHTML = __final_dis_val;
        document.getElementById("_final_igst_per").innerHTML = __final_igst_per;
        document.getElementById("_final_igst_val").innerHTML = __final_igst_val;
        document.getElementById("_final_cgst_per").innerHTML = __final_cgst_per;
        document.getElementById("_final_cgst_val").innerHTML = __final_cgst_val;
        document.getElementById("_final_sgst_per").innerHTML = __final_sgst_per;
        document.getElementById("_final_sgst_val").innerHTML = __final_sgst_val;
        document.getElementById("_final_total").innerHTML = __final_total;

        var submit1 = "";
        document.getElementById("Enter_Inv_No").innerHTML = submit1;
        var myRows = [];
        var headersText = [];
        var $headers = $("#selected_units th");
        var $rows = $("#selected_units tbody #data").each(function (index) {
            $cells = $(this).find("td");
            myRows[index] = {};

            $cells.each(function (cellIndex) {
                if (headersText[cellIndex] === undefined) {
                    headersText[cellIndex] = $($headers[cellIndex]).text();
                }
                myRows[index][headersText[cellIndex]] = $(this).text();
            });
        });

        var myObj = {
            "myrows": myRows
        };
        $.ajax({
            url: '/NewPurchase/Add_PO_to_DB',
            data: JSON.stringify(myRows),
            type: 'POST',
            contentType: 'application/json;',
            dataType: 'json',
            success: function (result) {
                //alert(result.ItemList[0].Str);
                var modal2 = document.getElementById("myModal2");
                modal2.style.display = "block";
                var data = result;
                var vno = '<p style="background-color:yellow;width:20%">Your PO.No is ' + data + '<p/>';
                document.getElementById("voucher_view").innerHTML = "Your PO.No is " + data + "";
                var submit = '<p style="color:red;">Submitted Successfully !!!<p/>';
                document.getElementById("submit_view").innerHTML = submit;
                setTimeout(function () {
                    window.location.reload(1);
                }, 3000);
            }
        });

        }
});

$('input[name=IGST]').change(function () {
    var igst = document.getElementById("IGST").value;
    if (igst != "") {
        $('#CGST').prop('disabled', true);
        $('#SGST').prop('disabled', true);
    }
    else {
        $('#CGST').prop('disabled', false);
        $('#SGST').prop('disabled', false);
    }
});
$('input[name=CGST]').change(function () {
    var cgst = document.getElementById("CGST").value;
    if (cgst != "") {
        $('#IGST').prop('disabled', true);
    }
    else {
        $('#IGST').prop('disabled', false);
    }
});
$('input[name=SGST]').change(function () {
    var sgst = document.getElementById("SGST").value;
    if (sgst != "") {
        $('#IGST').prop('disabled', true);
    }
    else {
        $('#IGST').prop('disabled', false);
    }
});
$('input[name=Final_Discount]').change(function () {
    var subtotal = parseFloat(document.getElementById("final_SubTotal").value);
    var Discount = parseFloat(document.getElementById("total_discount").value);
    var IGST = parseFloat(document.getElementById("total_igst").value);
    var CGST = parseFloat(document.getElementById("total_cgst").value);
    var SGST = parseFloat(document.getElementById("total_sgst").value);
    var discount = document.getElementById("Final_Discount").value;
    if (discount > 0) {
        var final_discount = (subtotal * discount) / 100;
        var final_total = (subtotal - final_discount) + IGST + CGST + SGST;
        document.getElementById("total_discount").value = final_discount.toFixed(2);
        document.getElementById("Final_Total").value = final_total.toFixed(2);
    }
    else {
        var _discount = document.getElementById("total_discount").value;
        var final_total = document.getElementById("Final_Total").value;
        document.getElementById("Final_Total").value = (final_total + _discount).toFixed(2);
        document.getElementById("total_discount").value = "0.00";
    }
});
$('input[name=Total_IGST]').change(function () {
    var subtotal = parseFloat(document.getElementById("final_SubTotal").value);
    var Discount = parseFloat(document.getElementById("total_discount").value);
    var IGST = parseFloat(document.getElementById("total_igst").value);
    var CGST = parseFloat(document.getElementById("total_cgst").value);
    var SGST = parseFloat(document.getElementById("total_sgst").value);
    var igst = document.getElementById("Total_IGST").value;
    if (igst > 0) {
        $('#Total_CGST').prop('disabled', true);
        $('#Total_SGST').prop('disabled', true);
        document.getElementById("total_cgst").value = "0.00";
        document.getElementById("total_sgst").value = "0.00";
        if (subtotal == 0) {
            document.getElementById("total_igst").value = "0.00";
        }
        else {
            var final_igst = (subtotal * igst) / 100;
            var final_total = (subtotal - Discount) + final_igst + CGST + SGST;
            document.getElementById("total_igst").value = final_igst.toFixed(2);
            document.getElementById("Final_Total").value = final_total.toFixed(2);
        }
    }
    else {
        $('#Total_CGST').prop('disabled', false);
        $('#Total_SGST').prop('disabled', false);
        var _igst = document.getElementById("total_igst").value;
        var final_total = document.getElementById("Final_Total").value;
        document.getElementById("Final_Total").value = (final_total - _igst).toFixed(2);
        document.getElementById("total_igst").value = "0.00";
    }
});
$('input[name=Total_CGST]').change(function () {
    var subtotal = parseFloat(document.getElementById("final_SubTotal").value);
    var Discount = parseFloat(document.getElementById("total_discount").value);
    var IGST = parseFloat(document.getElementById("total_igst").value);
    var CGST = parseFloat(document.getElementById("total_cgst").value);
    var SGST = parseFloat(document.getElementById("total_sgst").value);
    var cgst = document.getElementById("Total_CGST").value;
    if (cgst > 0) {
        document.getElementById("total_igst").value = "0.00";
        $('#Total_IGST').prop('disabled', true);
        if (subtotal == 0) {
            document.getElementById("total_cgst").value = "0.00";
        }
        else {
            var final_cgst = (subtotal * cgst) / 100;
            var final_total = (subtotal - Discount) + IGST + final_cgst + SGST;
            document.getElementById("total_cgst").value = final_cgst.toFixed(2);
            document.getElementById("Final_Total").value = final_total.toFixed(2);
        }
    }
    else {
        $('#Total_IGST').prop('disabled', false);
        var _cgst = document.getElementById("total_cgst").value;
        var final_total = document.getElementById("Final_Total").value;
        document.getElementById("Final_Total").value = (final_total - _cgst).toFixed(2);
        document.getElementById("total_cgst").value = "0.00";
    }
});
$('input[name=Total_SGST]').change(function () {
    var subtotal = parseFloat(document.getElementById("final_SubTotal").value);
    var Discount = parseFloat(document.getElementById("total_discount").value);
    var IGST = parseFloat(document.getElementById("total_igst").value);
    var CGST = parseFloat(document.getElementById("total_cgst").value);
    var SGST = parseFloat(document.getElementById("total_sgst").value);
    var sgst = document.getElementById("Total_SGST").value;
    if (sgst > 0) {
        document.getElementById("total_igst").value = "0.00";
        $('#Total_IGST').prop('disabled', true);
        if (subtotal == 0) {
            document.getElementById("total_sgst").value = "0.00";
        }
        else {
            var final_sgst = (subtotal * sgst) / 100;
            var final_total = (subtotal - Discount) + IGST + CGST + final_sgst;
            document.getElementById("total_sgst").value = final_sgst.toFixed(2);
            document.getElementById("Final_Total").value = final_total.toFixed(2);
        }
    }
    else {
        $('#Total_IGST').prop('disabled', false);
        var _sgst = document.getElementById("total_sgst").value;
        var final_total = document.getElementById("Final_Total").value;
        document.getElementById("Final_Total").value = (final_total - _sgst).toFixed(2);
        document.getElementById("total_sgst").value = "0.00";
    }
});

document.addEventListener('keydown', function (event) {
    if (event.keyCode === 9) {
        var Inv_No = $('#Ref_No').val();
        var model = new Object();
        model.Part_to_Descp = document.getElementById("Part_No").value;

        jQuery.ajax({
            type: "POST",
            url: "@Url.Action('Partno_to_Descp')",
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify({ name: model }),
            success: function (data) {
                const button = document.getElementById("Insert");
                if (Inv_No == "") {
                    var qty_err = '<p style="color:red;">Enter Reference No !!!<p/>';
                    document.getElementById("Enter_Inv_No").innerHTML = qty_err;
                    return;
                }
                else if (data[1] == "") {
                    button.disabled = true;
                    var inv = "";
                    document.getElementById("Enter_Inv_No").innerHTML = inv;
                }

                else {
                    button.disabled = false;
                    var inv = "";
                    document.getElementById("Enter_Inv_No").innerHTML = inv;
                }
                document.getElementById("value").innerHTML = data[0];
            },
            failure: function (errMsg) {
                alert(errMsg);
            }
        });
    }
});

// All the Below functions are for Product Entry and Product Listing

var modal = document.getElementById("myModal");
var modal1 = document.getElementById("myModal1");
var btn = document.getElementById("myBtn");
var btn1 = document.getElementById("addproduct");
var span = document.getElementById("close1");
var span1 = document.getElementById("close2");

btn.onclick = function () {
    modal.style.display = "block";
}
btn1.onclick = function () {
    modal1.style.display = "block";
}
span.onclick = function () {
    modal.style.display = "none";
}
span1.onclick = function () {
    modal1.style.display = "none";
    document.getElementById("Add_Name").value = "";
    document.getElementById("Add_Package").value = "";
    document.getElementById("Add_Value").value = "";
    document.getElementById("Add_PartNo").value = "";
    document.getElementById("Add_Description").value = "";
    document.getElementById("Add_Cost").value = "";
    document.getElementById("Add_MRP").value = "";
    document.getElementById("Add_SellPrice").value = "";
}
window.onclick = function (event) {
    if (event.target == modal) {
        modal.style.display = "none";
    }
}
window.onclick = function (event) {
    if (event.target == modal1) {
        modal1.style.display = "none";
        document.getElementById("Add_Name").value = "";
        document.getElementById("Add_Package").value = "";
        document.getElementById("Add_Value").value = "";
        document.getElementById("Add_PartNo").value = "";
        document.getElementById("Add_Description").value = "";
        document.getElementById("Add_Cost").value = "";
        document.getElementById("Add_MRP").value = "";
        document.getElementById("Add_SellPrice").value = "";
    }
}

btn.addEventListener("click", list);
span.removeEventListener("click", list);
window.removeEventListener("click", list);

function list() {
    document.addEventListener("keyup", function (event) {
        var package = document.querySelector("#Package_letter").value;
        var value = document.querySelector("#Value_letter").value;
        var partno = document.querySelector("#Partno_letter").value;
        var model = new Object();
        model.Package_letter = package;
        model.Value_letter = value;
        model.Partno_letter = partno;
        $.ajax({
            url: '/NewPurchase/PM_List',
            data: JSON.stringify({ name: model }),
            type: 'POST',
            contentType: 'application/json;',
            dataType: 'json',
            success: function (result) {
                $("#pm_list").find("tr:not(:first)").remove();
                for (var i = 0; i <= result.length - 1; i++) {
                    var newrow = '<tr id="list" onclick="List_to_View()"><td>' + result[i].P_code + '</td><td>' + result[i].Package + '</td><td>' + result[i].Value + '</td><td>' + result[i].Part_No + '</td><td>' + result[i].Description + '</td><td style="text-align:right">' + result[i].P_Cost + '</td></tr>';
                    $('#pm_list tr:last').after(newrow);
                }
            }
        });
    })
}

function List_to_View() {
    var table = document.getElementById('pm_list');
    for (var i = 0; i < table.rows.length; i++) {
        table.rows[i].addEventListener('click', function () {
            var partno = this.cells[3].innerHTML;
            var descp = this.cells[4].innerHTML;
            var cost = this.cells[5].innerHTML;
            document.getElementById("Part_No").value = partno;
            document.getElementById("value").innerHTML = descp;
            document.getElementById("Purchase_Rate").value = cost;
            modal.style.display = "none";
        });
    }
}

function Add_Prod() {
    var Name = document.getElementById("Add_Name").value;
    var Group = document.getElementById("Add_Group").value;
    var Mfr = document.getElementById("Add_Manufacturer").value;
    var pkg = document.getElementById("Add_Package").value;
    var value = document.getElementById("Add_Value").value;
    var PartNo = document.getElementById("Add_PartNo").value;
    var Descp = document.getElementById("Add_Description").value;
    var Cost = document.getElementById("Add_Cost").value;
    var MRP = document.getElementById("Add_MRP").value;
    var SellPrice = document.getElementById("Add_SellPrice").value;

    if (Name == "") {
        var qty_err = '<p style="color:red;">Field Product Name is required !!!<p/>';
        document.getElementById("message").innerHTML = qty_err;
        return;
    }
    else if (PartNo == "") {
        var qty_err = '<p style="color:red;">Field Part No is required !!!<p/>';
        document.getElementById("message").innerHTML = qty_err;
        return;
    }
    var model = new Object();
    model.Add_Name = Name;
    model.Add_Group = Group;
    model.Add_Manufacturer = Mfr;
    model.Add_PartNo = PartNo.toUpperCase();
    model.Add_Description = Descp;
    model.Add_Cost = Cost;
    model.Add_MRP = MRP;
    model.Add_Package = pkg;
    model.Add_Value = value;
    model.Add_SellPrice = SellPrice;
    $.ajax({
        url: "@Url.Action('Add_Product')",
        data: JSON.stringify({ name: model }),
        type: 'POST',
        contentType: 'application/json;',
        dataType: 'json',
        success: function (result) {
            if (result != "") {
                var qty_err = '<p style="color:red;">Part No already exists !!!<p/>';
                document.getElementById("message").innerHTML = qty_err;
                return;
            }
            else {
                var qty_err = '<p style="color:red;">Submitted Successfully !!!<p/>';
                document.getElementById("message").innerHTML = qty_err;
                return;
            }
        }
    });
}