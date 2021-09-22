async function getTimes() {
    var error = document.getElementById("error");
    error.innerText = "";
    var appointment_time = document.getElementById("Appointment_Time");
    if (appointment_time.length > 0) {
        for (var i = appointment_time.length - 1; i > -1; i--) {
            appointment_time.remove(i);
        }
    }
    var date;
    if (document.getElementById("Appointment_Date").value == "") {
        date = new Date(0);
    }
    else {
        var values = document.getElementById("Appointment_Date").value.split("-")
        for (var i = 0; i < values.length; i++) {
            values[i] = parseInt(values[i]);
        }
        date = new Date(values[0], values[1] - 1, values[2]);
    }
    var reference = new Date();
    var now = new Date(reference.getFullYear(), reference.getMonth(), reference.getDate());
    if (date >= now) {
        var url = window.location.origin + "/Appointments/GetTimes";
        var data = {
            dentistId: document.getElementById("Appointment_DentistId").value,
            date: document.getElementById("Appointment_Date").value,
            durationInMinutes: document.getElementById("Appointment_DurationInMinutes").value
        };
        var list;
        await $.get(url, data, obj => list = JSON.parse(obj));

        if (list == null || list.length == 0) {
            appointment_time.disabled = true;
            document.getElementById("Appointment_Confirm").disabled = true;
            if (list == null) {
                error.innerText = "Houve um problema de solicitação!";
            }
            else {
                error.innerText = "Não há horários disponíveis neste dia!";
            }
        }
        else {
            for (var i = 0; i < list.length; i++) {
                var option = await document.createElement("option");
                option.value = list[i];
                option.text = list[i];
                appointment_time.appendChild(option);
            }
            appointment_time.disabled = false;
            document.getElementById("Appointment_Confirm").disabled = false;
        }
    }
    else {
        appointment_time.disabled = true;
        document.getElementById("Appointment_Confirm").disabled = true;
        error.innerText = "Data inválida!";
    }

}

