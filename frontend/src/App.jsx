import "./App.css";
import { useEffect } from "react";
import { BrowserRouter as Router, Routes, Route, useLocation, useNavigate } from "react-router-dom";
import Home from './pages/Home/Home';
import Register from "./pages/Register/Register";
import Login from "./pages/Login/Login";
import Profile from "./pages/Profile/Profile";

function App() {
  return (
    <Router>
      <Main />
    </Router>
  );
}

function Main() {
  const navigate = useNavigate();
  const location = useLocation();

  useEffect(() => {
    if (localStorage.getItem("token") === null && sessionStorage.getItem("token") === null && location.pathname !== "/register") {
      navigate("/");
    }
  }, [navigate, location.pathname]);

  return (
    <Routes>
      <Route exact path="/" element={<Login />} />
      <Route exact path="/register" element={<Register />} />
      <Route exact path="/home" element={<Home />} />
      <Route exact path="/profile" element={<Profile />} />
    </Routes>
  );
}

export default App;
