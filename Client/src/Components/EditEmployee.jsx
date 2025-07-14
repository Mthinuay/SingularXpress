import React, { useState, useEffect } from "react";
import { useLocation } from "react-router-dom";
import MenuBar from "./MenuBar";
import "../Navy.css";
import {
  editEmployee,
  deriveDOBFromIdNumber,
  formatDateForDisplay,
  toISOStringSafe,
    showConfirmationToast,
} from "../Employee";

import { toast } from "react-toastify";

// MOCK
const getCurrentUserRole = () => {
  return "superuser";
};

const EditEmployee = () => {
  const location = useLocation();
  const [activeTab, setActiveTab] = useState("Personal");
  const [isEditable, setIsEditable] = useState(false);
  const [userRole, setUserRole] = useState(null);
  const [loading, setLoading] = useState(false);
  const [formErrors, setFormErrors] = useState({});

  // Track original ID and DOB loaded from DB
  const [originalIdNumber, setOriginalIdNumber] = useState("");
  const [originalDateOfBirth, setOriginalDateOfBirth] = useState("");

  const [employeeData, setEmployeeData] = useState({
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
    disability: false,
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

const getInitials = (firstName, middleName, lastName) => {
  let initials = "";

  if (firstName) initials += firstName.charAt(0).toUpperCase();

  if (middleName && middleName.trim().toLowerCase() !== "n/a") {
    initials += middleName.charAt(0).toUpperCase();
  }

  if (lastName) initials += lastName.charAt(0).toUpperCase();

  return initials;
};


  // Set user role and load employee data on mount or location.state change
useEffect(() => {
  const role = getCurrentUserRole();
  setUserRole(role);

  if (location.state) {
    const employee = location.state;
    const transformed = {
      ...employee,
      dateOfBirth: employee.dateOfBirth
        ? formatDateForDisplay(employee.dateOfBirth)
        : "",
      startDate: employee.startDate
        ? formatDateForDisplay(employee.startDate)
        : "",
      initials: getInitials(employee.firstName, employee.middleName, employee.lastName),
    };

    setEmployeeData(transformed);
    setOriginalIdNumber(employee.idNumber);
    setOriginalDateOfBirth(employee.dateOfBirth);
  }
}, [location.state]);

useEffect(() => {
  const initials = getInitials(
    employeeData.firstName,
    employeeData.middleName,
    employeeData.lastName
  );
  if (employeeData.initials !== initials) {
    setEmployeeData((prev) => ({
      ...prev,
      initials,
    }));
  }
}, [employeeData.firstName, employeeData.middleName, employeeData.lastName]);

useEffect(() => {
  // Maiden Name validation & styling logic
  if (
    employeeData.gender.toLowerCase() === "female" &&
    employeeData.maritalStatus.toLowerCase() === "married"
  ) {
    // Maiden name is required and should not be "N/A"
    if (!employeeData.maidenName || employeeData.maidenName === "N/A") {
      setFormErrors((prev) => ({
        ...prev,
        maidenName: "Maiden Name is required for married women.",
      }));
    } else {
      setFormErrors((prev) => {
        const { maidenName, ...rest } = prev;
        return rest;
      });
    }
  } else {
    // Clear maidenName error if condition doesn't apply
    setFormErrors((prev) => {
      const { maidenName, ...rest } = prev;
      return rest;
    });
    // For non-females or unmarried, set maidenName to "N/A" if not already set
    if (employeeData.maidenName !== "N/A") {
      setEmployeeData((prev) => ({ ...prev, maidenName: "N/A" }));
    }
  }
}, [employeeData.gender, employeeData.maritalStatus, employeeData.maidenName]);

// Disability validation & styling logic
useEffect(() => {
  if (employeeData.disability) {
    if (
      !employeeData.disabilityType ||
      employeeData.disabilityType === "N/A"
    ) {
      setFormErrors((prev) => ({
        ...prev,
        disabilityType: "Disability Type is required when Disability is 'Yes'.",
      }));
    } else {
      setFormErrors((prev) => {
        const { disabilityType, ...rest } = prev;
        return rest;
      });
    }
  } else {
    // Clear error if disability is false
    setFormErrors((prev) => {
      const { disabilityType, ...rest } = prev;
      return rest;
    });
    if (employeeData.disabilityType !== "N/A") {
      setEmployeeData((prev) => ({
        ...prev,
        disabilityType: "N/A",
      }));
    }
  }
}, [employeeData.disability, employeeData.disabilityType]);

useEffect(() => {
  setEmployeeData((prev) => {
    const updated = { ...prev };

    if (isEditable) {
      if (updated.maidenName === "N/A") {
        updated.maidenName = "";
      }
      // Clear disabilityType if editable and disability is true and disabilityType is "N/A"
      if (updated.disability && updated.disabilityType === "N/A") {
        updated.disabilityType = "";
      }
    } else {
      // Reset disabilityType to "N/A" if not editable and disability is false or disabilityType is empty
      if (!updated.disability && !updated.disabilityType) {
        updated.disabilityType = "N/A";
      }
    }

    return updated;
  });
}, [isEditable, employeeData.disability]);




  // Handle maidenName logic based on gender and maritalStatus
  useEffect(() => {
    if (employeeData.gender.toLowerCase() === "Female") {
      if (employeeData.maritalStatus.toLowerCase() === "Single") {
        // Set maidenName to lastName if single female
        if (employeeData.maidenName !== employeeData.lastName) {
          setEmployeeData((prev) => ({ ...prev, maidenName: prev.lastName }));
          setFormErrors((prev) => {
            const { maidenName, ...rest } = prev;
            return rest;
          });
        }
      } else if (
        employeeData.maritalStatus.toLowerCase() === "divorced" &&
        !employeeData.maidenName
      ) {
        setFormErrors((prev) => ({
          ...prev,
          maidenName: "Required for divorced women",
        }));
      } else {
        // Clear maidenName error if conditions no longer apply
        setFormErrors((prev) => {
          const { maidenName, ...rest } = prev;
          return rest;
        });
      }
    } else {
      // For non-females, set maidenName to "N/A" and clear errors
      if (employeeData.maidenName !== "N/A") {
        setEmployeeData((prev) => ({ ...prev, maidenName: "N/A" }));
      }
      setFormErrors((prev) => {
        const { maidenName, ...rest } = prev;
        return rest;
      });
    }
  }, [
    employeeData.gender,
    employeeData.maritalStatus,
    employeeData.lastName,
    employeeData.maidenName,
  ]);

  // Reset disabilityType if disability is false
  useEffect(() => {
    if (!employeeData.disability && employeeData.disabilityType !== "N/A") {
      setEmployeeData((prev) => ({
        ...prev,
        disabilityType: "N/A",
      }));
    }
  }, [employeeData.disability, employeeData.disabilityType]);

  if (userRole !== "superuser") {
    return (
      <div style={{ padding: 20, color: "red" }}>
        Access Denied. Only super users can access this page.
      </div>
    );
  }
  
const handleEditSaveClick = async () => {
  if (!isEditable) {
    setIsEditable(true);
    return;
  }

  const confirmed = await showConfirmationToast("Are you sure you want to save changes?");
  if (!confirmed) {
    setIsEditable(false); // Revert back to "Edit Profile"
    return;
  }

  if (formErrors.maidenName) {
    toast.error("Fix validation errors first.");
    return;
  }

  const idNumberTrimmed = employeeData.idNumber.trim();

  const payload = {
    ...employeeData,
    idNumber: idNumberTrimmed,
    dateOfBirth:
      idNumberTrimmed === originalIdNumber
        ? originalDateOfBirth
        : toISOStringSafe(employeeData.dateOfBirth),
    startDate: toISOStringSafe(employeeData.startDate),
  };

  try {
    setLoading(true);
    await editEmployee(payload.employeeNumber, payload);

    toast.success("Employee updated successfully!");
    setIsEditable(false);
    setOriginalIdNumber(payload.idNumber);
    setOriginalDateOfBirth(payload.dateOfBirth);
  } catch (error) {
    if (error.response?.data) {
      const serverErrors = error.response.data.errors || {};
      const generalMessage = error.response.data.message || "Validation failed";

      setFormErrors(serverErrors);
      toast.error(generalMessage);
      Object.entries(serverErrors).forEach(([field, message]) =>
        toast.error(`${field}: ${message}`)
      );
    } else {
      toast.error("Could not update employee. Please try again.");
    }
  } finally {
    setLoading(false);
  }
};

  const handleInputChange = (e) => {
    const { id, value, type, checked } = e.target;

    setEmployeeData((prevData) => {
      const updatedData = {
        ...prevData,
        [id]: type === "checkbox" ? checked : value,
      };

      // If ID number changes, update dateOfBirth as well
      if (id === "idNumber" && value.length >= 6) {
        const derivedDOB = deriveDOBFromIdNumber(value);
        if (derivedDOB) {
          updatedData.dateOfBirth = derivedDOB;
        }
      }

      // Handle disability field specially to store boolean
      if (id === "disability") {
        updatedData.disability = value === "yes";
        if (value !== "yes") {
          updatedData.disabilityType = "N/A";
        }
      }

      return updatedData;
    });
  };


  return (
    <div className="edit-employee-background">
      <MenuBar />

      <div className="edit-employee-heading-row">
        {[
          "Personal",
          "Career",
          "Leave",
          "Tax Profile",
          "Payroll",
          "Documents",
        ].map((tab) => (
          <div
            key={tab}
            className={`heading-item ${activeTab === tab ? "selected" : ""}`}
            onClick={() => setActiveTab(tab)}
          >
            {tab}
          </div>
        ))}
      </div>

      <div className="edit-button-container">
        <button className="edit-button" onClick={handleEditSaveClick} disabled={loading}>
          {isEditable ? "Save" : "Edit Profile"}
        </button>
      </div>

      <div className="edit-employee-top-container">
        <div className="photo-block">
          <img
            src={employeeData.documentPath || "/default-profile.png"}
            alt="Employee"
          />
        </div>
        <div className="photo-text-container">
          <div className="title">{`${employeeData.firstName} ${employeeData.lastName}`}</div>
          <div className="subtitle">{employeeData.jobTitle}</div>
        </div>
      </div>

      <div className="edit-employee-form-container">
        <div className="custom-header">Personal Information</div>

        <div className="sub-container">
          <div className="fields-container">
            {[
              ["Employee Number*", "employeeNumber"],
              ["Title", "title"],
              ["Initials", "initials"],
              ["ID Number*", "idNumber"],
              ["Nationality*", "nationality"],
              ["Citizenship*", "citizenship"],
            ].map(([label, id]) => (
              <div className="field" key={id}>
                <label className="field-label" htmlFor={id}>
                  {label}
                </label>
                <input
                  className="field-input"
                  id={id}
                  type="text"
                  value={employeeData[id]}
                  onChange={handleInputChange}
                  readOnly={!isEditable}
                />
              </div>
            ))}
          </div>

          <div className="fields-container">
            {[
              ["Marital Status*", "maritalStatus"],
              ["Date of Birth", "dateOfBirth"],
              ["Preferred Name", "preferredName"],
              ["Gender", "gender"],
            ].map(([label, id]) => (
              <div className="field" key={id}>
                <label className="field-label" htmlFor={id}>
                  {label}
                </label>
             <input
  className="field-input"
  id={id}
  type="text"
  value={
    id === "maritalStatus" && employeeData[id]
      ? employeeData[id].charAt(0).toUpperCase() + employeeData[id].slice(1)
      : employeeData[id]
  }
  onChange={handleInputChange}
  readOnly={!isEditable}
/>

              </div>
            ))}

            <div className="field">
              <label className="field-label" htmlFor="disability">
                Disability
              </label>
              <select
                className="field-input"
                id="disability"
                value={employeeData.disability ? "yes" : "no"}
                onChange={handleInputChange}
                disabled={!isEditable}
              >
                <option value="no">No</option>
                <option value="yes">Yes</option>
              </select>
            </div>

            <div className="field">
              <label className="field-label" htmlFor="disabilityType">
                Disability Type
              </label>
    <input
  className="field-input"
  id="disabilityType"
  type="text"
  value={employeeData.disabilityType}
  onChange={handleInputChange}
  readOnly={!isEditable || employeeData.disabilityType === "N/A"}
  style={
    // Only apply blue background if NOT editable or value is "N/A" and not required
    (!isEditable || employeeData.disabilityType === "N/A") &&
    !employeeData.disability
      ? { backgroundColor: "#C7D9E5" }
      : {}
  }
/>

            </div>
          </div>

          <div className="fields-container row-3">
            {[
              ["First Name", "firstName"],
              ["Middle Name", "middleName"],
              ["Last Name", "lastName"],
            ].map(([label, id]) => (
              <div className="field" key={id}>
                <label className="field-label" htmlFor={id}>
                  {label}
                </label>
                <input
                  className="field-input"
                  id={id}
                  type="text"
                  value={employeeData[id]}
                  onChange={handleInputChange}
                  readOnly={!isEditable}
                />
              </div>
            ))}
          </div>

          <div className="fields-container row-3">
            {[
              ["Maiden Name", "maidenName"],
              ["Contact Number*", "contactNumber"],
              ["Email", "email"],
            ].map(([label, id]) => (
<div className="field" key={id}>
  <label className="field-label" htmlFor={id}>
    {label}
    {formErrors[id] && <span style={{ color: "red" }}> *</span>}
  </label>

  <input
    className="field-input"
    id={id}
    type={id === "email" ? "email" : "text"}
    value={employeeData[id]}
    onChange={handleInputChange}
    readOnly={
      !isEditable ||
      (id === "disabilityType" && (!employeeData.disability || employeeData.disabilityType === "N/A"))
    }
    style={
      (id === "maidenName" && (!isEditable || employeeData.maidenName === "N/A")) ||
      (id === "disabilityType" && (!isEditable || employeeData.disabilityType === "N/A"))
        ? { backgroundColor: "#C7D9E5" }
        : {}
    }
  />

  {formErrors[id] && (
    <div className="error-text" style={{ color: "red" }}>
      {formErrors[id]}
    </div>
  )}
</div>


            ))}
          </div>
        </div>
      </div>
    </div>
  );
};

export default EditEmployee;
