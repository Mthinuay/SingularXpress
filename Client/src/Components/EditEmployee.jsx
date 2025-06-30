import React, { useState } from "react";
import MenuBar from "./MenuBar";
import "./Navy.css";

const EditEmployee = () => {
  const [activeTab, setActiveTab] = useState("Personal");

  const tabs = [
    "Personal",
    "Career",
    "Leave",
    "Tax Profile",
    "Payroll",
    "Documents",
  ];

  return (
    <div className="edit-employee-background">
      <MenuBar />
    
      <div className="edit-employee-heading-row">
        {tabs.map((tab) => (
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
        <button className="edit-button">Edit Profile</button>
      </div>
     {/* Top grid container with photo block */}
      <div className="edit-employee-top-container">
        <div className="photo-block">
          {/* Replace src with actual employee photo URL */}
          <img
            src="https://via.placeholder.com/241x185.png?text=Photo"
            alt="Employee"
            style={{ width: "100%", height: "100%", objectFit: "cover" }}
          />
        </div>

          {/* Name container next to photo */}
<div className="photo-text-container">
  <div className="title">Wesley Armstrong</div>
  <div className="subtitle">Subtitle Text Here</div>
</div>

        {/* Add other grid items here if needed */}
      </div>
      <div className="edit-employee-form-container">
        <div className="custom-header">
          Personal Information
        </div>

        <div className="sub-container">
          {/* Row 1 */}
          <div className="fields-container">
            <div className="field">
              <label className="field-label" htmlFor="field1">Employee Number*</label>
              <input className="field-input" id="field1" type="text" />
            </div>
            <div className="field">
              <label className="field-label" htmlFor="field2">Title</label>
              <input className="field-input" id="field2" type="text" />
            </div>
            <div className="field">
              <label className="field-label" htmlFor="field3">Initials</label>
              <input className="field-input" id="field3" type="text" />
            </div>
            <div className="field">
              <label className="field-label" htmlFor="field4">ID Number*</label>
              <input className="field-input" id="field4" type="number" />
            </div>
            <div className="field">
              <label className="field-label" htmlFor="field5">Nationality*</label>
              <input className="field-input" id="field5" type="text" />
            </div>
            <div className="field">
              <label className="field-label" htmlFor="field6">Citizenship*</label>
              <input className="field-input" id="field6" type="text" />
            </div>
          </div>

          {/* Row 2 (exact same style) */}
          <div className="fields-container">
            <div className="field">
              <label className="field-label" htmlFor="field7">Marital Status*</label>
              <input className="field-input" id="field7" type="text" />
            </div>
            <div className="field">
              <label className="field-label" htmlFor="field8">Date of Birth</label>
              <input className="field-input" id="field8" type="text" />
            </div>
            <div className="field">
              <label className="field-label" htmlFor="field9">Preferred Name</label>
              <input className="field-input" id="field9" type="tel" />
            </div>
            <div className="field">
              <label className="field-label" htmlFor="field10">Gender</label>
              <input className="field-input" id="field10" type="email" />
            </div>
          <div className="field">
  <label className="field-label" htmlFor="field11">Disability</label>
  <select className="field-input" id="field11">
    <option value="">Select</option>
    <option value="yes">Yes</option>
    <option value="no">No</option>
  </select>
</div>

            <div className="field">
              <label className="field-label" htmlFor="field12">Disability type</label>
              <input className="field-input" id="field12" type="text" />
            </div>
          </div>

  {/* Row 3 - 3 fields */}
    <div className="fields-container row-3">
    <div className="field">
      <label className="field-label" htmlFor="field13">First Name</label>
      <input className="field-input" id="field13" type="text" />
    </div>
    <div className="field">
      <label className="field-label" htmlFor="field14">Middle Name</label>
      <input className="field-input" id="field14" type="text" />
    </div>
    <div className="field">
      <label className="field-label" htmlFor="field15">Last Name</label>
      <input className="field-input" id="field15" type="text" />
    </div>
  </div>
{/* Row 4 - 3 fields */}
  <div className="fields-container row-3">
    <div className="field">
      <label className="field-label" htmlFor="field16">Maiden Name</label>
      <input className="field-input" id="field16" type="text" />
    </div>
    <div className="field">
      <label className="field-label" htmlFor="field17">Contact Number*</label>
      <input className="field-input" id="field17" type="number" />
    </div>
    <div className="field">
      <label className="field-label" htmlFor="field18">Email</label>
      <input className="field-input" id="field18" type="email" />
    </div>
  </div>
        </div>
      </div>

      <div className="edit-employee-content"></div>
    </div>
  );
};

export default EditEmployee;
