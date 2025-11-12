import { urlencoded } from "express";
import Pagination from "./Pagination";

export default class Http {
    private func = (e: any, d: Pagination | null) => {};
    private funcE = (e: string) => {};
    private funcF = () => {};
    private running: boolean = true;

    private response: any = null;
    private error: string = "";
    private page: Pagination | null = null;

    static Get<T = object>(url: string, params?: Object): Http {
        if (!url.includes("?"))
            url += "?";

        if (params != undefined)
            url += this.#GetParams(params);

        let task = fetch(url, {
            method: "GET",
            headers: { "Content-Type": "application/json" },
        });

        const h = new Http();
        h.#CheckResponse<T>(url, task);
        return h;
    }

    static Post<T = object>(url: string, body?: Object): Http {
        let task = fetch(url, {
            method: "POST",
            body: body != null ? JSON.stringify(body) : '',
            headers: { "Content-Type": "application/json" }
        });

        const h = new Http();
        h.#CheckResponse<T>(url, task);
        return h;
    }

    static Put<T = object>(url: string, body: Object): Http {
        let task = fetch(url, {
            method: "PUT",
            body: JSON.stringify(body),
            headers: { "Content-Type": "application/json" }
        });

        const h = new Http();
        h.#CheckResponse<T>(url, task);
        return h;
    }

    static Delete<T = object>(url: string, params: Object): Http {
        if (!url.includes("?"))
            url += "?";

        url += this.#GetParams(params);

        let task = fetch(url, {
            method: "DELETE",
            headers: { "Content-Type": "application/json" }
        });

        const h = new Http();
        h.#CheckResponse<T>(url, task);
        return h;
    }

    async #CheckResponse<T>(url: string, task: Promise<Response>) {
        let resp: Response;

        try {
            resp = await task;
        }
        catch {
            throw "Fail to connect on server.";
        }

        let txt = await resp.text();

        if (!resp.ok) {
            if (txt.length > 0) {
                let ob = JSON.parse(txt);
                let msg = (ob.message as string | null) ?? "";
                if (msg.length > 0){
                    this.error = msg;
                    this.funcE(this.error);
                    this.funcF();
                    this.running = false;
                    return;
                }
            }
            this.error = "Erro on GET " + url;
            this.funcE(this.error);
            this.funcF();
            this.running = false;
            return;
        }

        const data = JSON.parse(txt).data as T;
        if(JSON.parse(txt).pagination != null)
            this.page = JSON.parse(txt).pagination as Pagination;

        this.func(data, this.page);
        this.funcF();
        this.running = false;
    }

    then<T>(func = (data: T | null, pagination: Pagination | null): void => {}){

        if(this.running)
            this.func = func;
        else if(this.response != null)
            func(this.response, this.page);

        return this;
    }

    catch(func = (error: string) => {}){
        if(this.running)
            this.funcE = func;
        else if(this.error.length > 0)
            func(this.error);

        return this;
    }

    finally(func = () => {}){
        if(this.running)
            this.funcF = func;
        else
            func();
    }

    static #GetParams(ob: Record<string, any>): string {
        let result: string = "";

        for (let n of Object.getOwnPropertyNames(ob)) {
            let d = ob[n];
            if (typeof d == "string")
                result += `${n}=${encodeURIComponent(d)}&`;
            else
                result += `${n}=${encodeURIComponent(JSON.stringify(d))}&`;
        }

        if (result.length > 1)
            result = result.substring(0, result.length - 1);

        return result;
    }
}