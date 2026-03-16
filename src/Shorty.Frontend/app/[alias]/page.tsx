import { notFound, redirect } from "next/navigation";
import { Metadata } from "next";
import { HIDDEN_ROBOTS } from "@/lib/metadata";

type Props = {
    params: Promise<{alias:string}>;
}

export async function generateMetadata({params}:Props) : Promise<Metadata> {
    const {alias} = await params;

    return {
        title: `Redirecting...`,
        description: `Redirecting you to the destination for ${alias}`,
        robots: HIDDEN_ROBOTS,
    };
}

export default async function RedirectPage({ params }: Props) {
    const {alias} = await params;
    console.log(alias);

    let res: Response;

    try {
        res = await fetch(`${process.env.ENGINE_BASE_URL}${process.env.REDIRECT_PATH}/${alias}`, {
            redirect: "manual",
            cache: 'no-store',
        });
    } catch (err) {
        console.error("server error: ", err)
        throw err
    }

    if (res.status == 500) {
        const data = await res.json().catch(() => ({}));
        const errorMsg = data?.detail || "internal server error";
        
        console.error("server error: ", errorMsg)
        throw new Error(errorMsg);
    }

    if (res.status == 404) {
        notFound();
    }

    if (res.status == 303) {
        const destination = res.headers.get("location");
        if (destination) {
            console.log("redirecting to: ", destination);
            redirect(destination);
        }
    }
}
