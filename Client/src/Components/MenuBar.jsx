import "../MenuBar.css";
import React, { useState } from "react";
import { useNavigate } from "react-router-dom";

const MenuBar = ({ onMenuClick }) => {
  const [reportOpen, setReportOpen] = useState(false);
  const [companyOpen, setCompanyOpen] = useState(false);
  const navigate = useNavigate();

  const toggleReport = () => {
    setReportOpen(!reportOpen);
  };

  const toggleCompany = () => {
    setCompanyOpen(!companyOpen);
  };

  return (
    <div className="menu-bar-container">
      <div className="menu-inner">
        <div className="menu-logo-wrapper">
          <span className="menu-bar-logo-text-bold">singular</span>
          <span className="menu-bar-logo-text-light">express</span>
        </div>

        <ul className="menu-list">
          <li>
            <div className="menu-item-wrapper">
              <img
                src="/images/contacts_product.png"
                alt="Personal icon"
                className="menu-icon"
              />
              <span className="menu-heading">Personal</span>
            </div>
          </li>

          <li>
            <div className="menu-item-wrapper" onClick={toggleReport}>
              <img
                src="/images/cases.png"
                alt="Employee Management icon"
                className="menu-icon"
              />
              <span className="menu-heading">
                Employee Management
                <span className="menu-dropdown">{reportOpen ? "▲" : "▼"}</span>
              </span>
            </div>
            {reportOpen && (
              <ul className="submenu">
                <li>
                  <span
                    className="menu-subitem"
                    onClick={() => navigate("/addEmployee")}
                  >
                    Add Employee
                  </span>
                </li>
              </ul>
            )}
          </li>

          <li>
            <div className="menu-item-wrapper" onClick={toggleCompany}>
              <img
                src="/images/autostop.png"
                alt="Company Management icon"
                className="menu-icon"
              />
              <span className="menu-heading">
                Company Management
                <span className="menu-dropdown">{companyOpen ? "▲" : "▼"}</span>
              </span>
            </div>
            {companyOpen && (
              <ul className="submenu">
                <li>
                  <span className="menu-subitem">Add Heading here</span>
                </li>
              </ul>
            )}
          </li>

          <li>
            <div className="menu-item-wrapper">
              <img
                src="/images/regular_expression.png"
                alt="Payroll Management icon"
                className="menu-icon"
              />
              <span className="menu-heading">Payroll Management</span>
            </div>
          </li>

          <li>
            <div className="menu-item-wrapper">
              <img
                src="/images/savings.png"
                alt="Document Management icon"
                className="menu-icon"
              />
              <span className="menu-heading">Document Management</span>
            </div>
          </li>

          <li>
            <div className="menu-item-wrapper">
              <img
                src="/images/attach_file.png"
                alt="Admin Management tools icon"
                className="menu-icon"
              />
              <span className="menu-heading">Admin Management tools</span>
            </div>
          </li>
        </ul>
      </div>

      <div className="menu-footer">
        <img
          src="/images/setitngs_icon.png"
          alt="Settings icon"
          className="menu-icon"
        />
      </div>
    </div>
  );
};

export default MenuBar;
