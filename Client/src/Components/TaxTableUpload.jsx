import React, { useState, useRef, useEffect } from "react";
import axios from "axios";
import "./TaxTableUpload.css";

const financialYears = [
  { value: "2022-2023", label: "March 2022 - April 2023" },
  { value: "2023-2024", label: "March 2023 - April 2024" },
  { value: "2024-2025", label: "March 2024 - April 2025" },
];

export default function TaxTableUpload() {
  const [year, setYear] = useState("");
  const [file, setFile] = useState(null);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [isUploading, setIsUploading] = useState(false);

  const fileInputRef = useRef(null);

  useEffect(() => console.log("[DEBUG] Year changed:", year), [year]);
  useEffect(() => console.log("[DEBUG] File changed:", file), [file]);

  const handleYearChange = (e) => {
    setError("");
    setSuccess("");
    setYear(e.target.value);
  };

  const handleAutoUpload = async (e) => {
    setError("");
    setSuccess("");

    const selected = e.target.files[0];
    if (!selected) {
      setFile(null);
      return;
    }

    const ext = selected.name.split(".").pop().toLowerCase();
    if (!["xls", "xlsx"].includes(ext)) {
      setError("Only Excel files (.xls, .xlsx) are allowed.");
      setFile(null);
      fileInputRef.current.value = "";
      return;
    }

    if (!year) {
      setError("Please select a financial year before uploading.");
      fileInputRef.current.value = "";
      return;
    }

    setFile(selected);
    try {
      setIsUploading(true);
      const formData = new FormData();
      formData.append("Year", year);
      formData.append("File", selected);

      const response = await axios.post(
        "https://localhost:7137/api/tax-tables/upload",
        formData,
        {
          headers: { "Content-Type": "multipart/form-data" },
          timeout: 10000,
        }
      );

      setSuccess(response.data.message || "Upload successful.");
      setYear("");
      setFile(null);
      fileInputRef.current.value = "";
    } catch (err) {
      console.error("[DEBUG] Auto-upload failed:", err);
      setError(err.response?.data?.message || "Upload failed.");
    } finally {
      setIsUploading(false);
    }
  };

  return (
    <div className="full-screen-bg">
      <div className="center-frame">
        <div className="content-centered">
          <div className="headings-container">
            <div className="center-logo">
              <span className="center-logo-text-bold">singular</span>
              <span className="center-logo-text-light">express</span>
            </div>
            <h1 className="upload-title">Upload tax table</h1>
            <p className="file-type-text">Only Excel files (.xls, .xlsx) are supported.</p>

            <div className="gender-select-wrapper">
              <select
                className="name-input"
                value={year}
                onChange={handleYearChange}
              >
                <option value="" disabled>
                  Please select the year
                </option>
                {financialYears.map((yr) => (
                  <option key={yr.value} value={yr.value}>
                    {yr.label}
                  </option>
                ))}
              </select>
            </div>

            {error && <p className="error-message">{error}</p>}
            {success && <p className="success-message">{success}</p>}
          </div>

          <div className="upload-section">
            <p className="drop-files-text">Drop files here</p>
            <p className="or-text">or</p>

            <div className="gender-select-wrapper">
              <label className="upload-file-button">
                {isUploading ? "Uploading..." : "Upload file"}
                <input
                  type="file"
                  accept=".xls,.xlsx"
                  ref={fileInputRef}
                  onChange={handleAutoUpload}
                  className="upload-button"
                />
              </label>

              {file && (
                <p style={{ color: "#002D40", fontSize: "13px", marginTop: "8px" }}>
                  Selected: <strong>{file.name}</strong>
                </p>
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
