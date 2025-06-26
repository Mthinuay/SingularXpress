
import MenuBar from "./MenuBar"; // adjust path if needed
import "./Navy.css";

const EditEmployee = () => {
  return (
    <>
      <MenuBar />
      <div className="corner-fill-top" />
      <div className="corner-fill-bottom" />

      <div className="edit-employee-background">
        {/* Top container with grid */}
        <div className="edit-employee-top-container">
          <div className="grid-item item1">Personal</div>
          <div className="grid-item item2">Career</div>
          <div className="grid-item item3">Leave</div>
          <div className="grid-item item4">Tax Profile</div>
          <div className="grid-item item5">Payroll Results</div>
          <div className="grid-item item6">Documents</div>
        </div>

        <div className="edit-employee-content">
          <h1>Edit Employee</h1>
          {/* Rest of your editable form */}
        </div>
      </div>
    </>
  );
};

export default EditEmployee;
