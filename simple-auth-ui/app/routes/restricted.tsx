import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router'

const API_URL = import.meta.env.VITE_API_URL ?? 'https://localhost:5001'

export default function Restricted() {
    const navigate = useNavigate()
    const [userEmail, setUserEmail] = useState<string>('')
    const [loading, setLoading] = useState(true)

    useEffect(() => {
        const token = localStorage.getItem('token')

        if (!token) {
            navigate('/login')
            return
        }

        // Decode JWT untuk mendapatkan email
        try {
            const payload = JSON.parse(atob(token.split('.')[1]))

            const email = payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress']
                || payload.email
                || 'User'

            // Ambil user ID
            const id = payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier']
                || payload.sub
                || ''

            console.log(payload);
            setUserEmail(email || 'User')
        } catch (err) {
            console.error('Failed to decode token:', err)
            navigate('/login')
        } finally {
            setLoading(false)
        }
    }, [navigate])

    if (loading) {
        return (
            <div className="flex min-h-screen items-center justify-center bg-slate-50 px-4">
                <div className="text-center">
                    <div className="inline-block h-8 w-8 animate-spin rounded-full border-4 border-solid border-indigo-600 border-r-transparent"></div>
                </div>
            </div>
        )
    }

    return (
        <div className="flex min-h-screen items-center justify-center bg-slate-50 px-4">
            <div className="w-full max-w-md rounded-2xl bg-white p-8 shadow-lg">
                <div className="text-center">
                    {/* Icon/Logo */}
                    <div className="mx-auto mb-4 flex h-16 w-16 items-center justify-center rounded-full bg-indigo-100">
                        <svg className="h-8 w-8 text-indigo-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                        </svg>
                    </div>

                    <h1 className="text-2xl font-semibold text-slate-900">
                        Hello
                    </h1>
                    <p className="mt-2 text-lg text-slate-600">
                        {userEmail}
                    </p>

                    <div className="mt-6 border-t border-slate-200"></div>

                    <p className="mt-4 text-sm text-slate-500">
                        Welcome Back
                    </p>
                </div>
            </div>
        </div>
    )
}