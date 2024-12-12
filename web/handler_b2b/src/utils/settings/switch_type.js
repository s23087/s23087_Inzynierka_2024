"use server"

import getDbName from "../auth/get_db_name";
import getUserId from "../auth/get_user_id"
import logout from "../auth/logout";

/**
 * Sends post request to local api. If answer is positive it will change user app type from solo to organization and logout the user. If fails it return string with error message.
 * @return {Promise<{error: boolean, message: string}>}
 */
export default async function changeTypeToOrganization(){
    try {
        const dbName = await getDbName();
        const userId = await getUserId();
        let url = `${process.env.API_DEST}/${dbName}/Settings/switch`;
        let response = await fetch(url, {
            method: "POST",
            body:{
                userId: userId
            }
        });

        if (response.ok){
            logout()
            return {
                error: false,
                message: "",
            }
        }

        return {
            error: true,
            message: "Error: Server could not succeeded yor request.",
        }
    } catch (error) {
        console.error(error)
        console.error("failed fetch at ChangeTypeToOrganization")
        return {
            error: true,
            message: "Error: Could not connect to server.",
        }
    }
}