// Components/AddEmployee.js
import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { toast } from "react-toastify";
import "../Navy.css";

import {
  addEmployee,
  validateRequiredFields,
  validateEmail,
  handleFileChange,
  handleInputChange,
  fetchAllEmployees,
  toISOStringSafe,
  populateFromIdNumber,
  showConfirmationToast,
} from "../Employee";



const getCurrentUserRole = () => "superuser";

const AddEmployee = () => {
  const navigate = useNavigate();
  const [userRole, setUserRole] = useState(null);
  const [formErrors, setFormErrors] = useState({});
  const [uploading, setUploading] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");
  const [employeesList, setEmployeesList] = useState([]);
  const [loading, setLoading] = useState(false);
  const [idNumberError, setIdNumberError] = useState("");
  const [touched, setTouched] = React.useState({});
  const [reportsToOptions, setReportsToOptions] = useState([]);
  const [allEmployees, setAllEmployees] = useState([]);

  const [employee, setEmployee] = useState({
    employeeNumber: "",
    firstName: "",
    lastName: "",
    maidenName: "",
    title: "",
    dateOfBirth: "",
    initials: "",
    idType: "id",
    idNumber: "",
    preferredName: "",
    gender: "",
    middleName: "",
    contactNumber: "",
    nationality: "",
    citizenship: "",
    disability: "",
    disabilityType: "",
    email: "",
    maritalStatus: "",
    homeAddress: "",
    city: "",
    postalCode: "",
    startDate: "",
    department: "",
    jobTitle: "",
    employeeStatus: "",
    reportsTo: "",
    documentPath: "",
  });

useEffect(() => {
  const fetchEmployees = async () => {
    try {
      const data = await fetchAllEmployees(); // uses your existing API handler
      setAllEmployees(data); // Store full employee data
    } catch (err) {
      console.error("Failed to fetch employees:", err);
    }
  };

  fetchEmployees();
}, []);

useEffect(() => {
  const query = employee.reportsTo.trim().toLowerCase();

  if (query.length < 2) {
    setReportsToOptions([]);
    return;
  }

const matches = allEmployees
  .filter(emp =>
    `${emp.firstName} ${emp.lastName}`.toLowerCase().includes(query)
  )
  .map(emp => `${emp.firstName} ${emp.lastName}`);


  setReportsToOptions(matches);
}, [employee.reportsTo, allEmployees]);


useEffect(() => {
  console.log("AddEmployee rendered");
  console.log("loading:", loading);
}, [loading]);

  useEffect(() => {
    setUserRole(getCurrentUserRole());
  }, []);

  useEffect(() => {
    fetchAllEmployees().then(setEmployeesList).catch(console.error);
  }, []);

  if (userRole !== "superuser") {
    return (
      <div style={{ padding: 20, color: "red" }}>
        Access Denied. Only super users can access this page.
      </div>
    );
  }

const handleSave = async () => {
  const confirmed = await showConfirmationToast("Are you sure you want to save changes?");
  if (!confirmed) return;

  const validation = validateRequiredFields(employee);
  const emailError = validateEmail(employee.email);
  if (emailError) {
    validation.errors.email = emailError;
    validation.isValid = false;
  }
  setFormErrors(validation.errors);
  if (!validation.isValid) return;

  const payload = {
    ...employee,
     disability: employee.disability === "yes" ? true : false,
    dateOfBirth: toISOStringSafe(employee.dateOfBirth),
    startDate: toISOStringSafe(employee.startDate),
  };

  try {
    setLoading(true);
    console.log("Employee details:", payload)
    const saved = await addEmployee(payload);
    toast.success("Employee saved successfully!");
    navigate("/editEmployee", { state: saved });
  } catch (error) {
    if (error.response?.data) {
      const serverErrors = error.response.data.errors || {};
      const generalMessage = error.response.data.message || "Validation failed";
      setFormErrors(serverErrors);
      toast.error(generalMessage);
      Object.entries(serverErrors).forEach(([field, msg]) => toast.error(`${field}: ${msg}`));
    } else {
      toast.error("Failed to save employee. Please try again.");
    }
  } finally {
    setLoading(false);
  }
};

function onBlurField(e) {
  const { name } = e.target;
  setTouched(prev => ({ ...prev, [name]: true }));
}

  // UPDATED onInputChange
  const onInputChange = (e) => {
    const { name, value } = e.target;
        let newValue = value;

      if (name === "disability") {

    newValue = value === "yes"; // boolean conversion
  }
    if (["contactNumber", "postalCode"].includes(name)) {
      if (/\D/.test(value)) {
        setFormErrors(prev => ({ ...prev, [name]: "Only numbers allowed" }));
        return;
      } else {
        setFormErrors(prev => ({ ...prev, [name]: null }));
      }
    }
  

    if (name === "idNumber") {
      if (value.length <= 13) {
        if (value.length === 13) {
          const derived = populateFromIdNumber(value);
          setEmployee(prev => ({
            ...prev,
            idNumber: value,
            dateOfBirth: derived.dateOfBirth || "",
            gender: derived.gender || "",
            nationality: derived.nationality || "",
            citizenship: derived.citizenship || "",
          }));
        } else {
          setEmployee(prev => ({
            ...prev,
            idNumber: value,
            dateOfBirth: "",
            gender: "",
            nationality: "",
            citizenship: "",
          }));
        }
      }
      return;
    }

    if (name === "dateOfBirth") return;

    handleInputChange(e, employee, setEmployee, setIdNumberError, employeesList);
  };

  const onFileChange = (e) =>
    handleFileChange(e, setEmployee, setUploading, setErrorMessage);

  const idPlaceholder = employee.idType === "passport" ? "Passport Number" : "ID Number";


return (
  <div className="full-screen-bg">
    {/* Background shapes and layout */}
    <div className="shape-1"></div>
    <div className="shape-2"></div>
    <div className="shape-3"></div>
    <div className="shape-4"></div>
    <div className="shape-5"></div>

    <div className="center-frame">
      <div className="left-frame">
        <div className="left-frame-centered">
          <div className="headings-container">
            <div className="center-logo">
              <span className="center-logo-text-bold">singular</span>
              <span className="center-logo-text-light">express</span>
            </div>
            <h1 className="new-employee-title">New Employee</h1>
            <p className="new-employee-subtitle">
              Please complete the form below to add a new
              <br />
              <span className="employee-word">employee.</span>
            </p>
          </div>
        </div>
       <div className="left-frame">
        <div className="personal-details-container">
          <div className="personal-details-heading">
            <span>Personal</span> <span>Details</span>
          </div>
        </div>

        <div className="name-surname-container">
          {/* First & Last Name */}
<input
  type="text"
  placeholder={touched.firstName && formErrors.firstName ? formErrors.firstName : "First Name"}
  className={`name-input ${touched.firstName && formErrors.firstName ? "input-error" : ""}`}
  name="firstName"
  value={employee.firstName}
  onChange={onInputChange}
  onBlur={onBlurField}
/>

<input
  type="text"
  placeholder={formErrors.lastName || "Last Name"}
  className={`name-input ${formErrors.lastName ? "input-error" : ""}`}
  name="lastName"
  value={employee.lastName}
  onChange={onInputChange}
/>


          {/* ID Type & ID Number */}
          <div className="input-row">
            <div className="gender-select-wrapper">
              <select
                className="name-input gender-select"
                name="idType"
                value={employee.idType}
                onChange={onInputChange}
              >
                <option value="id">ID Number</option>
                <option value="passport">Passport Number</option>
              </select>
              <img
                src="/images/arrow_drop_down_circle.png"
                alt="Dropdown icon"
                className="dropdown-icon"
              />
            </div>

        <input
  type="text"
  placeholder={formErrors.idNumber || idPlaceholder}
  className={`name-input ${formErrors.idNumber ? "input-error" : ""}`}
  name="idNumber"
  value={employee.idNumber}
  onChange={onInputChange}
/>

          </div>
        

          {/* DOB - read-only */}
          <input
            type="text"
            placeholder="Birthday"
            className="name-input"
            name="dateOfBirth"
            value={employee.dateOfBirth}
            disabled
          />

{/* Disability */}
<div className="input-row">
  <div className="gender-select-wrapper">
<select
  className="name-input gender-select"
  name="disability"
  value={employee.disability}
  onChange={onInputChange}
>
  <option value="" disabled>
    Do you have a disability?
  </option>
  <option value="yes">Yes</option>
  <option value="no">No</option>
</select>

    <img
      src="/images/arrow_drop_down_circle.png"
      alt="Dropdown icon"
      className="dropdown-icon"
    />
  </div>

  {employee.disability === true && (
    <input
      type="text"
      placeholder="Disability Type"
      className="name-input"
      name="disabilityType"
      value={employee.disabilityType || ""}
      onChange={onInputChange}
    />
  )}
</div>


          {/* Contact Number (numeric only) */}
       <input
  type="text"
  placeholder={formErrors.contactNumber || "Contact Number"}
  className={`name-input ${formErrors.contactNumber ? "input-error" : ""}`}
  name="contactNumber"
  value={employee.contactNumber}
  onChange={onInputChange}
/>


          {/* Marital Status */}
          <div className="gender-select-wrapper">
            <select
              className="name-input gender-select"
              name="maritalStatus"
              value={employee.maritalStatus}
              onChange={onInputChange}
            >
              <option value="" disabled>
                Marital Status
              </option>
              <option value="single">Single</option>
              <option value="married">Married</option>
              <option value="divorced">Divorced</option>
              <option value="widow">Widow</option>
              <option value="widower">Widower</option>
            </select>
            <img
              src="/images/arrow_drop_down_circle.png"
              alt="Dropdown icon"
              className="dropdown-icon"
            />
          </div>

          {/* Address */}
              <input
            type="text"
            placeholder={formErrors.homeAddress || "Home address"}
            className={`name-input ${formErrors.homeAddress ? "input-error" : ""}`}
            name="homeAddress"
            value={employee.homeAddress}
            onChange={onInputChange}
          />

          {/* City & Postal Code (numeric) */}
          <div className="city-postal-container">
                 <input
              type="text"
              placeholder={formErrors.city || "City"}
              className={`name-input city-input ${formErrors.city ? "input-error" : ""}`}
              name="city"
              value={employee.city}
              onChange={onInputChange}
            />
                 <input
              type="text"
              placeholder={formErrors.postalCode || "Postal Code"}
              className={`name-input postal-input ${formErrors.postalCode ? "input-error" : ""}`}
              name="postalCode"
              value={employee.postalCode}
              onChange={onInputChange}
            />
          </div>
     

          {/* Gender (auto-derived, disabled) */}
          <div className="gender-select-wrapper">
            <select
              className="name-input gender-select"
              name="gender"
              value={employee.gender}
              disabled
            >
              <option value="" disabled>
                Gender
              </option>
              <option value="male">Male</option>
              <option value="female">Female</option>
            </select>
            <img
              src="/images/arrow_drop_down_circle.png"
              alt="Dropdown icon"
              className="dropdown-icon"
            />
          </div>
        </div>
      </div>
      </div>

      {/* Right frame */}
      <div className="right-frame">
        <div className="right-form-container">
          <div className="right-frame-content">
            <div className="name-surname-container">
              <div className="personal-details-container">
                <div className="personal-details-heading">
                  <span>Company</span> <span>Details</span>
                </div>
              </div>
  <div className="form-group">
  <label htmlFor="startDate" className="form-label">
    Employee Start Date
  </label>
  <input
    type="date"
    id="startDate"
    name="startDate"
    value={employee.startDate}
    onChange={onInputChange}
    className={`name-input ${formErrors.startDate ? "input-error" : ""}`}
  />
</div>


              {/* Department */}
              <div className="gender-select-wrapper">
                <select
                  className="name-input gender-select"
                  name="department"
                  value={employee.department}
                  onChange={onInputChange}
                >
                  <option value="" disabled>Select Department</option>
                  <option value=" Cape Town ">Cape Town</option>
                  <option value="Johannesburg">Johannesburg</option>
                  <option value="Share Trust">Share Trust</option>
                  <option value="Financial Services">Financial Services</option>
                </select>
                <img src="/images/arrow_drop_down_circle.png" alt="Dropdown icon" className="dropdown-icon" />
              </div>

              {/* Employee Code - disabled */}
              <input
                type="text"
                className="name-input"
                name="employeeNumber"
                value={employee.employeeNumber}
                placeholder="Employee Code"
                disabled
              />

                  {/* Job Title */}
          <input
            type="text"
            placeholder={formErrors.jobTitle || "Job Title"}
            className={`name-input ${formErrors.jobTitle ? "input-error" : ""}`}
            name="jobTitle"
            value={employee.jobTitle}
            onChange={onInputChange}
          />

              {/* Employee Status */}
          <div className="gender-select-wrapper">
            <select
              name="employeeStatus"
              value={employee.employeeStatus}
              className={`name-input gender-select ${formErrors.employeeStatus ? "input-error" : ""}`}
              onChange={onInputChange}
            >
                  <option value="" disabled>Select Employee Status</option>
                  <option value="Permanent">Permanent</option>
                  <option value="Fixed-term contract">Fixed-term contract</option>
                </select>
                <img src="/images/arrow_drop_down_circle.png" alt="Dropdown icon" className="dropdown-icon" />
              </div>

           {/* Reports To */}
<input
  list="reportsToList"
  placeholder={formErrors.reportsTo || "Reports to"}
  className={`name-input ${formErrors.reportsTo ? "input-error" : ""}`}
  name="reportsTo"
  value={employee.reportsTo}
  onChange={onInputChange}
/>

<datalist id="reportsToList">
  {reportsToOptions.map((name, index) => (
    <option key={index} value={name} />
  ))}
</datalist>



              {/* Email */}
          <input
  type="text"
  placeholder={formErrors.email || "Work email address"}
  className={`name-input blue-text ${formErrors.email ? "input-error" : ""}`}
  name="email"
  value={employee.email}
  onChange={onInputChange}
/>

           

              {/* File Upload */}
              <div className="gender-select-wrapper">
                <input
                  type="file"
                  accept=".jpg,.jpeg"
                  className="name-input"
                  id="documentUpload"
                  name="documentPath"
                  onChange={onFileChange}
                />
                <img src="/images/arrow_upload_ready.png" alt="Upload icon" className="dropdown-icon" />
              </div>
              {uploading && <div className="paceholder-text">Uploading...</div>}
              {errorMessage && (
                <div className="error-text" style={{ color: "red", marginTop: "4px" }}>
                  {errorMessage}
                </div>
              )}
              {employee.documentPath && (
                <div style={{ marginTop: "-10px" }}>
                  Uploaded file: <a href={employee.documentPath} target="_blank" rel="noreferrer">View Document</a>
                </div>
              )}

              {/* Save Button */}
              <button className="save-button" onClick={handleSave} disabled={loading}>
                {loading ? "Saving..." : "Save"}
              </button>

              <div className="right-frame-bottom">
                <p className="right-frame-bottom-text">
                  Please ask the employee to confirm their details after registration.
                </p>
                <p className="right-frame-bottom-text">
                  <span className="align-right">Privacy Policy | Terms & Conditions</span>
                  <br />
                  <span className="align-left">
                    Copyright © 2025 Singular Systems. All rights reserved.
                  </span>
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
);

};

export default AddEmployee;
