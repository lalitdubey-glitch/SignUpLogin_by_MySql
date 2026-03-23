$(document).ready(function () {

    GetAllUsers();

    $("input[required]").keyup(function () {
        if ($(this).val().trim() == "") {
            $(this).next("span").show();
            $(this).addClass("is-invalid");
        }
        else {
            $(this).next("span").hide();
            $(this).removeClass("is-invalid");
        }
    })
     

    $("#ShowHidePass").click(function () {
        if ($("#pass").attr("type") === "password")
        {
            $("#pass").attr("type", "text");
            $("#ShowHidePass").val("Hide");
        }
        else
        {
            $("#pass").attr("type", "password");
            $("#ShowHidePass").val("Show");
        }
    })

    $("#img").change(function () {
        var file = new FileReader();
        file.onload = function (e) {
            $("#ShowPrv").show();
            $("#ShowPrv").attr("src", e.target.result);
        }
        file.readAsDataURL(this.files[0]);
    })

    $("#save").on("click", function () {
        $("#save").val("Save")
        Save();
    })



    $(document).on("click", ".btnEdit", function () {
        var id = $(this).data("id"); 
        $("#save").val("Edit")
        $.ajax({
            url: "/home/GetOneUser",
            type: "get",
            data: { id: id },
            success: async function (res) {
                if (res.success == false) {
                    alert("No User Found");
                    return;
                }
                $.each(res, async function (index, item) {
                    $("#id").val(item.id);
                    $("#name").val(item.name);
                    $("#email").val(item.email).prop("readonly" , true);
                    $("#mob").val(item.mob);
                    $("#pass").attr("Placeholder" , "Leave blank if you dont want to change Password"); 
                    //$("#pass").prop("readonly" ,true); 
                    if (item.dob != null) {
                        $("#dob").val(item.dob.split('T')[0]);
                    }
                  
                    if (item.img != null) {
                        $("#ShowPrv").show();
                        $("#ShowPrv").attr("src", `/UserImg/${item.img}`);
                    }
                    if (item.gender === "male") {
                        $("#male").prop("checked", true);
                    }
                    else if (item.gender === "female") {
                        $("#female").prop("checked", true);
                    }
                    else if (item.gender === "other") {
                        $("#other").prop("checked", true);
                    }
                    
                    if (item.hobby != null) {
                        var hb = item.hobby.split(", ");
                        $("#cricket").prop("checked", hb.includes("cricket"));
                        $("#carrom").prop("checked", hb.includes("carrom"));
                        $("#chess").prop("checked", hb.includes("chess"));
                        $("#football").prop("checked", hb.includes("football"));
                    }
                     
                    $("#profession").val(item.profession);
                    $("#pincode").val(item.pincode);
                    await PincodeFetch();
                    $("#state").val(item.state);
                    $("#dist").val(item.dist);
                    $("#vill").val(item.vill);
                    
                }) 
            },
            error: function (res) {
                console.warn(res);
            }
        })
    })

    $(document).on("click", ".btnDelete", function () {
        var id = $(this).data("id");  
        $.ajax({
            url: "/home/DeleteUser",
            type: "post",
            data: { id: id },
            dataType:"text",
            success: function (res) {
                if (res.success == false) {
                    alert("User Not Deleted");
                    return;
                }
                console.log(res)
                GetAllUsers();
                alert("delete");
            },
            error: function (res,xhr) {
                console.warn(res);
                console.warn(xhr);
            }
        })
    })

    //Login

    $("#btnLogin").on("click", function () {
        var email = $("#email").val();
        var pass = $("#pass").val();
        var remeber = $("#remeber").is(":checked"); 
        $.ajax({
            url: "/home/Login",
            type: "post",
            data: { email: email, pass: pass, remeber: remeber },
            success: function (res) {
                if (res.success == false) {
                    alert("Email or Password is incorrect!");
                    return;
                }
                else {
                    alert("Welcome User");
                    $("#Loginform")[0].reset();
                    window.location.href = "/home/Dashboard/";
                }
            },
            error: function (res,xhr) {
                console.warn(res);
                console.warn(xhr);
            }
        })
    })

    $("#fetchPin").on("click", async function () {
        await PincodeFetch();
    });
    
})

async function PincodeFetch() {
    var pincode = $("#pincode").val().trim();
    const res = await fetch(`https://api.postalpincode.in/pincode/${pincode}`);
    const data = await res.json();   
    if (data[0].Status === "Success") {
        $("#state").val(data[0].PostOffice[0].State);
        $("#dist").val(data[0].PostOffice[0].District);
        $("#vill").html("<option selected disabled>Select Village</option>");
        $.each(data[0].PostOffice, function (index, item) {
            $("#vill").append(`<option value="${item.Name}">${item.Name}</option>`);
        });
    } else {
        $("#pincode").val("");
        alert("Wrong Pincode");
    }
}

function GetAllUsers() {
    $.ajax({
        url: "/home/GetAllUsersData",
        type: "get",
        success: function (res) {

            var t = $("#table tbody").empty();
             
            $.each(res, function (index, item) {
                t.append(`
                <tr>
                <td> ${index + 1}</td>
                <td> ${item.id}</td>
                <td> ${item.name}</td>
                <td> ${item.email}</td>
                <td> ${item.mob}</td>
                <td> ${item.gender}</td>
                <td> ${item.hobby != null ? item.hobby : ""}</td >
                <td> <img src="/UserImg/${item.img != null ? item.img:"default.jpg"}" style="object-fit: contain;" height="100px" width="100px"/></td> 
                <td> ${item.pincode}</td>
                <td> ${item.profession}</td>
                <td> ${item.state}</td>
                <td> ${item.dist}</td>
                <td> ${item.vill}</td>
                <td> ${item.dob != null ? item.dob.split('T')[0] : ""}</td>
                <td> ${item.Regdate}</td>
                <td> 
                <input type="button" value="Delete" data-id="${item.id}" class="btn btn-outline-danger btnDelete btn-sm"/>
                <input type="button" value="Edit" data-id="${item.id}" class="btn btn-outline-success btnEdit btn-sm"/>
                </td>
                </tr>
                `) 
            }) 
        },
        error: function (res) {
            console.warn(res)
        }
    })
}

//$("#btnLogout").on("click", function () {  
//    $.ajax({
//        url: "/home/Logout",
//        type: "get", 
//        success: function (res) { 
//            if (res.success == true) {
//                window.location.href = "/home/Login/";
//            } 
//        },
//        error: function (res) {
//            console.log(res);
//        }
//    })
//});
 

function Save() {
    if ($("input[required]").val().trim() == "") {
        $("input[required]").next("span").show();
        $("input[required]").addClass("is-invalid");
        return;
    }
    else {
        $("input[required]").next("span").hide();
        $("input[required]").removeClass("is-invalid");
    }
    var form = new FormData(document.getElementById("signup"));
    $.ajax({
        url: "/home/SignUp",
        type: "post",
        data: form,
        dataType: "text",
        contentType: false,
        processData: false,
        success: function (res) { 
            var r = JSON.parse(res);  
            if (r.exist == true) {
                alert("User Already Exist Please use Another Email..!");
                return;
            }
            else if (r.success == true) {
                document.getElementById("signup").reset();
                $("#ShowPrv").attr("src", "");
                $("#ShowPrv").hide();
                GetAllUsers();
            }
        },
        error: function (xhr) {
            console.warn(xhr);
        }

    })
}