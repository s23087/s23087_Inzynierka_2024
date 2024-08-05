"use client";

import Input from "../components/inputs/input";
import { useRouter } from "next/navigation";

export default function Login_Form() {
  const router = useRouter();

  return (
    <form className="grid gap-y-9" action="">
      <div className="grid gap-y-6">
        <Input
          input_type="email"
          input_name="email"
          input_placeholder="email"
        />
        <Input
          input_type="password"
          input_name="password"
          input_placeholder="password"
        />
      </div>
      <div className="grid gap-y-4 px-6">
        <button
          className="bg-main_blue-primary hover:bg-main_blue-primary_hover rounded-md shadow-md p-3"
          type="submit"
        >
          Sign in
        </button>
        <button
          className="bg-main_blue-secondary hover:bg-main_blue-secondary_hover rounded-md shadow-md p-3"
          onClick={() => router.push("/registration")}
        >
          Registration
        </button>
      </div>
    </form>
  );
}
