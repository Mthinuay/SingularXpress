// App.js

import { Routes, Route, useLocation } from "react-router-dom";
import MenuBar from "./Components/MenuBar";
import AddEmployee from "./Components/AddEmployee";
import EditEmployee from "./Components/EditEmployee"; // Make sure the path is correct
import "./MenuBar.css";

function App() {
  const location = useLocation();
  const isAddEmployeePage = location.pathname === "/addEmployee";

  return (
    <div className="App" style={{ display: "flex" }}>
      {/* Show MenuBar on all pages except addEmployee */}
      {!isAddEmployeePage && <MenuBar />}

      <div style={{ flex: 1 }}>
        <Routes>
          <Route path="/addEmployee" element={<AddEmployee />} />
          <Route path="/editEmployee" element={<EditEmployee />} />
          {/* Add other routes here */}
        </Routes>
      </div>
    </div>
  );
}

export default App;
