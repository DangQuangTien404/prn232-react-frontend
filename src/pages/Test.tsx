import ky, { type HTTPError } from "ky";
import { useEffect } from "react";

export default function Test() {
  useEffect(() => {
    (async () => {
      const url = "https://localhost:7035/api/Accounts/register";

      try {
        const data = await ky.post(url).json();
        console.log("ky success:", data);
      } catch (error) {
        const err = error as HTTPError;

        console.log(error);
        console.log(err.name);
        console.log(err.message);
        console.log("=======================");
        console.log(err.request);
        console.log(err.response);
        console.log("=======================");
        console.log(JSON.stringify(err, null, 4));
        console.log(await err.response.json());
      }
    })();
  }, []);

  return <div>TEST</div>;
}
