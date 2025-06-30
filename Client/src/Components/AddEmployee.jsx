import "./Navy.css";
import { useNavigate } from "react-router-dom";

const AddEmployee = () => {
  const navigate = useNavigate();

  const handleSave = () => {
    // Here you can also add form validation or save logic before navigating
    navigate("/editEmployee");
  };

  return (
    <div className="full-screen-bg">
      <div className="shape-1"></div>
      <div className="shape-2"></div>
      <div className="shape-3"></div>
      <div className="shape-4"></div>
      <div className="shape-5"></div>

      <div className="center-frame">
        <div className="left-frame">
          {/* Left column content */}

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
            <div class="personal-details-container">
              <div className="personal-details-heading">
                <span>Personal</span> <span>Details</span>
              </div>
            </div>
            <div className="name-surname-container">
              <input
                type="text"
                placeholder="Full Name & Surname"
                className="name-input"
              />
              <input
                type="text"
                placeholder="ID number"
                className="name-input"
              />
              <input
                type="text"
                placeholder="Birthday (DD/MM/YYYY)"
                className="name-input"
              />
              <input type="text" placeholder="Number" className="name-input" />
              <input
                type="text"
                placeholder="Marital status"
                className="name-input blue-text"
              />
              <input
                type="text"
                placeholder="Home address"
                className="name-input"
              />

              <div className="city-postal-container">
                <input
                  type="text"
                  placeholder="City"
                  className="name-input city-input"
                />
                <input
                  type="text"
                  placeholder="Postal Code"
                  className="name-input postal-input"
                />
              </div>
              <div className="gender-select-wrapper">
                <select className="name-input gender-select" defaultValue="">
                  <option value="" disabled>
                    Gender
                  </option>
                  <option value="male">Male</option>
                  <option value="female">Female</option>
                </select>
                <img
                  src="/images/arrow_drop_down_circle.png"
                  alt="Dropdown icon"
                  className="dropdown-icon "
                />
              </div>
            </div>

            {/* rest of your content like logo, titles, etc. */}
          </div>
        </div>

        <div className="right-frame">
          {/* Right column content */}
          <div className="right-form-container">
            <div className="right-frame-content">
              <div className="name-surname-container">
                <div class="personal-details-container">
                  <div className="personal-details-heading">
                    <span>Company</span> <span>Details</span>
                  </div>
                </div>
                <input
                  type="text"
                  placeholder="Employee Start Date (DD/MM/YYYY)"
                  className="name-input"
                />
                <div className="gender-select-wrapper">
                  <input
                    type="text"
                    placeholder="Department"
                    className="name-input grey-text"
                  />
                  <img
                    src="/images/arrow_drop_down_circle.png"
                    alt="Dropdown icon"
                    className="dropdown-icon"
                  />
                </div>

                <div className="gender-select-wrapper">
                  <input
                    type="text"
                    placeholder="Employee code"
                    className="name-input grey-text"
                  />
                  <img
                    src="/images/arrow_drop_down_circle.png"
                    alt="Dropdown icon"
                    className="dropdown-icon"
                  />
                </div>
                <input
                  type="text"
                  placeholder="Job Title"
                  className="name-input"
                />
                <div className="gender-select-wrapper">
                  <input
                    type="text"
                    placeholder="Employee Status"
                    className="name-input"
                  />
                  <img
                    src="/images/arrow_drop_down_circle.png"
                    alt="Dropdown icon"
                    className="dropdown-icon"
                  />
                </div>
                <input
                  type="text"
                  placeholder="Reports to"
                  className="name-input"
                />
                <input
                  type="text"
                  placeholder="Work email address"
                  className="name-input blue-text"
                />
                <div className="gender-select-wrapper">
                  <input
                    type="text"
                    placeholder="Document Upload"
                    className="name-input"
                  />
                  <img
                    src="/images/arrow_upload_ready.png"
                    alt="Dropdown icon"
                    className="dropdown-icon"
                  />

                  <button className="save-button" onClick={handleSave}>Save</button>
                  <div className="right-frame-bottom">
                    <p className="right-frame-bottom-text">
                      Please ask the employee to confirm their details after
                      registration.
                    </p>
                    <p className="right-frame-bottom-text">
                        <span className="align-right">Privacy Policy | Terms & Conditions</span>
                      <br />
                       <span className="align-left">Copyright © 2025 Singular Systems. All rights reserved.</span>
                    </p>
                  </div>
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
