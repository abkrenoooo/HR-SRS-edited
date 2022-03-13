const BtnSubmit = document.getElementById("btn-submit"),
    AddEmployee = document.getElementById("Add-employee"),
    Progress = document.getElementById("Progress"),
    GenralSettings = document.getElementById("Genral-Settings"),
    Premissions = document.getElementById("Premissions"),
    AddNewGroup = document.getElementById("Add-New-Group"),
    Attendance = document.getElementById("Attendance"),
    SalaryReport = document.getElementById("Salary-report");

//AddEmployee
AddEmployee.children[1].children[0].addEventListener('click', () => {
    AddEmployee.children[1].children[0].classList.toggle('true')
})
AddEmployee.children[2].children[0].addEventListener('click', () => {
    AddEmployee.children[2].children[0].classList.toggle('true')
})
AddEmployee.children[3].children[0].addEventListener('click', () => {
    AddEmployee.children[3].children[0].classList.toggle('true')
})
AddEmployee.children[4].children[0].addEventListener('click', () => {
    AddEmployee.children[4].children[0].classList.toggle('true')
})

//Progress
Progress.children[1].children[0].addEventListener('click', () => {
    Progress.children[1].children[0].classList.toggle('true')
})
Progress.children[2].children[0].addEventListener('click', () => {
    Progress.children[2].children[0].classList.toggle('true')
})
Progress.children[3].children[0].addEventListener('click', () => {
    Progress.children[3].children[0].classList.toggle('true')
})
Progress.children[4].children[0].addEventListener('click', () => {
    Progress.children[4].children[0].classList.toggle('true')
})

//Genral-Settings
GenralSettings.children[1].children[0].addEventListener('click', () => {
    GenralSettings.children[1].children[0].classList.toggle('true')
})
GenralSettings.children[2].children[0].addEventListener('click', () => {
    GenralSettings.children[2].children[0].classList.toggle('true')
})
GenralSettings.children[3].children[0].addEventListener('click', () => {
    GenralSettings.children[3].children[0].classList.toggle('true')
})
GenralSettings.children[4].children[0].addEventListener('click', () => {
    GenralSettings.children[4].children[0].classList.toggle('true')
})

//Premissions
Premissions.children[1].children[0].addEventListener('click', () => {
    Premissions.children[1].children[0].classList.toggle('true')
})
Premissions.children[2].children[0].addEventListener('click', () => {
    Premissions.children[2].children[0].classList.toggle('true')
})
Premissions.children[3].children[0].addEventListener('click', () => {
    Premissions.children[3].children[0].classList.toggle('true')
})
Premissions.children[4].children[0].addEventListener('click', () => {
    Premissions.children[4].children[0].classList.toggle('true')
})

//AddNewGroup
AddNewGroup.children[1].children[0].addEventListener('click', () => {
    AddNewGroup.children[1].children[0].classList.toggle('true')
})
AddNewGroup.children[2].children[0].addEventListener('click', () => {
    AddNewGroup.children[2].children[0].classList.toggle('true')
})
AddNewGroup.children[3].children[0].addEventListener('click', () => {
    AddNewGroup.children[3].children[0].classList.toggle('true')
})
AddNewGroup.children[4].children[0].addEventListener('click', () => {
    AddNewGroup.children[4].children[0].classList.toggle('true')
})

//Attendance
Attendance.children[1].children[0].addEventListener('click', () => {
    Attendance.children[1].children[0].classList.toggle('true')
})
Attendance.children[2].children[0].addEventListener('click', () => {
    Attendance.children[2].children[0].classList.toggle('true')
})
Attendance.children[3].children[0].addEventListener('click', () => {
    Attendance.children[3].children[0].classList.toggle('true')
})
Attendance.children[4].children[0].addEventListener('click', () => {
    Attendance.children[4].children[0].classList.toggle('true')
})

//SalaryReport
SalaryReport.children[1].children[0].addEventListener('click', () => {
    SalaryReport.children[1].children[0].classList.toggle('true')
})
SalaryReport.children[2].children[0].addEventListener('click', () => {
    SalaryReport.children[2].children[0].classList.toggle('true')
})
SalaryReport.children[3].children[0].addEventListener('click', () => {
    SalaryReport.children[3].children[0].classList.toggle('true')
})
SalaryReport.children[4].children[0].addEventListener('click', () => {
    SalaryReport.children[4].children[0].classList.toggle('true')
})




onclick = () => {
    if ((AddEmployee.children[1].children[0].classList.contains('true') || AddEmployee.children[2].children[0].classList.contains('true') ||
            AddEmployee.children[3].children[0].classList.contains('true') || AddEmployee.children[4].children[0].classList.contains('true')) && (Progress.children[1].children[0].classList.contains('true') || Progress.children[2].children[0].classList.contains('true') ||
            Progress.children[3].children[0].classList.contains('true') || Progress.children[4].children[0].classList.contains('true')) && (GenralSettings.children[1].children[0].classList.contains('true') || GenralSettings.children[2].children[0].classList.contains('true') ||
            GenralSettings.children[3].children[0].classList.contains('true') || GenralSettings.children[4].children[0].classList.contains('true')) && (Premissions.children[1].children[0].classList.contains('true') || Premissions.children[2].children[0].classList.contains('true') ||
            Premissions.children[3].children[0].classList.contains('true') || Premissions.children[4].children[0].classList.contains('true')) && (AddNewGroup.children[1].children[0].classList.contains('true') || AddNewGroup.children[2].children[0].classList.contains('true') ||
            AddNewGroup.children[3].children[0].classList.contains('true') || AddNewGroup.children[4].children[0].classList.contains('true')) && (Attendance.children[1].children[0].classList.contains('true') || Attendance.children[2].children[0].classList.contains('true') ||
            Attendance.children[3].children[0].classList.contains('true') || Attendance.children[4].children[0].classList.contains('true')) && (SalaryReport.children[1].children[0].classList.contains('true') || SalaryReport.children[2].children[0].classList.contains('true') ||
            SalaryReport.children[3].children[0].classList.contains('true') || SalaryReport.children[4].children[0].classList.contains('true'))) {

        BtnSubmit.classList.remove('display')
    } else {
        BtnSubmit.classList.add('display')
    }
}