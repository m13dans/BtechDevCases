import type { Route } from "./+types/home";
import { Link } from 'react-router'

export function meta({}: Route.MetaArgs) {
  return [
    { title: "Simple Auth App" },
    { name: "description", content: "Welcome to Simple Auth!" },
  ];
}


export default function Home() {
    return (
        <div className="flex min-h-screen items-center justify-center bg-slate-50 px-4">
            <div className="w-full max-w-md rounded-2xl bg-white p-8 shadow-lg">
                <div className="text-center">
                    {/* Title */}
                    <h1 className="text-3xl font-bold text-slate-900">
                        Welcome
                    </h1>
                    <p className="mt-2 text-sm text-slate-500">
                        Simple Authentication Demo
                    </p>

                    {/* Divider */}
                    <div className="my-6 border-t border-slate-200"></div>

                    {/* Action Buttons */}
                    <div className="flex flex-col gap-3">
                        <Link
                            to="/login"
                            className="w-full rounded-lg bg-indigo-600 px-4 py-2.5 text-sm font-semibold text-white transition hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2"
                        >
                            Log In
                        </Link>
                        <Link
                            to="/register"
                            className="w-full rounded-lg border border-slate-300 px-4 py-2.5 text-sm font-semibold text-slate-700 transition hover:bg-slate-50 focus:outline-none focus:ring-2 focus:ring-slate-500 focus:ring-offset-2"
                        >
                            Create Account
                        </Link>
                    </div>
                </div>
            </div>
        </div>
    )
}
