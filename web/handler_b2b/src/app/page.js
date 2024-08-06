import Image from "next/image";
import LoginForm from "./../components/Login_form/login_form";
import bigLogo from "../../public/big_logo.png";

export default function App() {
  return (
    <main className="d-flex h-100 w-100 justify-content-center align-items-center pb-5">
      <div className="container mb-5 px-5 mx-4">
        <div className="row mb-4">
          <div className="col text-center">
            <Image src={bigLogo} alt="Logo" />
          </div>
        </div>

        <div className="row mb-3">
          <div className="col">
            <LoginForm />
          </div>
        </div>
      </div>
    </main>
  );
}
