import { useEffect} from "react";
import { LockOutlined, UserOutlined } from "@ant-design/icons";
import { Button, Checkbox, Form, Input, message, Typography } from "antd";
import { useNavigate } from "react-router-dom";
import apiService from '../../ApiService';


const Login = () => {
    const navigate = useNavigate();
    const [messageApi, contextHolder] = message.useMessage();
    // const { login } = useContext(AuthContext);
  
    useEffect(() => {
      console.log('mpike');
    },[])
  
    const onFinish = async (values) => {
      console.log("Received values of form: ", values);
      try {
        const { username, password, remember } = values;
        const response = await apiService.login(username, password, remember);
        messageApi.open({
          type: 'success',
          content: 'Successfully logged in',
        });
        console.log(response.data.token);
        navigate("/home");
      } catch (error) {
        //check if error is 400
        console.log(error.response)
        if (error.response.status === 401) {
          //get the error message
          messageApi.open({
            type: 'error',
            content: error.response.data.validationErrors.username[0] 
          });
        }
        else {
          messageApi.open({
            type: 'error',
            content: 'There was an error while logging in'
          });
        }
      }
    };
  
    return (
      <div style={{paddingLeft: "50px" ,display: "flex", flexDirection: "column", justifyContent: "center", alignItems: "center", height: "85vh", width: "90vw"}}>
        <p className="title" style={{marginTop: "0px"}}>MovieMatch</p>
        <Typography.Title style={{marginBottom: "40px", color: "white"}}>Login</Typography.Title>
          <Form
          style={{width: '250px', height: '500px', margin: 'auto'}}
            name="normal_login"
            className="login-form"
            initialValues={{
              remember: true,
            }}
            onFinish={onFinish}
          >
            {contextHolder}
            <Form.Item
              name="username"
              rules={[
                {
                  required: true,
                  message: "Please input your Username!",
                },
              ]}
            >
              <Input
                prefix={<UserOutlined className="site-form-item-icon" />}
                placeholder="Username"
              />
            </Form.Item>
            <Form.Item
              name="password"
              rules={[
                {
                  required: true,
                  message: "Please input your Password!",
                },
              ]}
            >
              <Input
                prefix={<LockOutlined className="site-form-item-icon" />}
                type="password"
                placeholder="Password"
              />
            </Form.Item>
            <Form.Item style={{color: "white"}}>
              <Form.Item name="remember" valuePropName="checked" noStyle>
                <Checkbox style={{color: "white"}}>Remember me</Checkbox>
              </Form.Item>
              <span style={{ marginRight: "8px"}}>|</span>
              Or <a href="/Register">Register now!</a>
            </Form.Item>
            <Form.Item style={{display: "flex", flexDirection: "column", alignItems: "center"}}>
              <Button
                type="primary"
                htmlType="submit"
                className="login-form-button"
                style={{ marginRight: "5px" }}
              >
                Log in
              </Button>
            </Form.Item>
          
            <br />
            <br />
            {/* <Button type="primary" value="large" onClick={() => navigate('/Upload')}>Go to Upload</Button> */}
          </Form>
      </div>
    );
  };
  export default Login;