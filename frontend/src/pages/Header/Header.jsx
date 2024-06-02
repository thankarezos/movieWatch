import "../../App.css";
import { Badge, Image, Space, Typography, Avatar } from "antd";
import { UserOutlined } from "@ant-design/icons";
import { useState } from "react";
import { useNavigate } from "react-router-dom";

function Title() {
  const [isLoggedin, setIsLoggedin] = useState(false);
  const navigate = useNavigate();
  return (
    <>
      <div className="header">
        <Image width={130} preview={false}></Image>
        <p className="title">MovieMatch</p>

        <Avatar
          onClick={() => {isLoggedin ? navigate("/home") : navigate("/")}}
          icon={<UserOutlined />}
          // src="https://api.dicebear.com/7.x/miniavs/svg?seed=1"
          style={{
            boxShadow: "0px 0px 10px 2px rgba(219, 0, 0, 0.515)",
            width: "35px",
            height: "35px",
            backgroundColor: "rgba(255, 255, 255, 0.5)",
            margin: "14px 40px 0px",
            cursor: "pointer",
          }}
        />
      </div>
    </>
  );
}

export default Title;
