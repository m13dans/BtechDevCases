import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router'

const API_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5000'

interface RestrictedResponse {
    email?: string
    userId?: string | number
}

export default function Restricted() {
    const navigate = useNavigate()

    const [userEmail, setUserEmail] = useState('')
    const [loading, setLoading] = useState(true)
    const [error, setError] = useState('')

    useEffect(() => {
        const validateAuthentication = async () => {
            const token = localStorage.getItem('token')

            // Tidak ada token
            if (!token) {
                navigate('/login', { replace: true })
                return
            }

            try {
                const response = await fetch(`${API_URL}/api/restricted`, {
                    method: 'GET',
                    headers: {
                        Authorization: `Bearer ${token}`,
                        'Content-Type': 'application/json',
                    },
                })

                if (response.status === 401) {
                    localStorage.removeItem('token')
                    navigate('/login', { replace: true })
                    return
                }

                if (response.status === 403) {
                    setError('You do not have permission to access this page.')
                    return
                }

                if (!response.ok) {
                    throw new Error(`API request failed: ${response.status}`)
                }

                // Ambil token baru dari response header
                const newAccessToken = response.headers.get('X-New-Access-Token')

                if(!newAccessToken) {
                    localStorage.removeItem('token')
                    console.error('Access token not found in response headers. Redirecting to login.')
                    navigate('/login', { replace: true })
                    return
                }

                localStorage.setItem('token', newAccessToken)
                console.log('Access token refreshed')

                try {
                    const payload = JSON.parse(
                        atob(newAccessToken!.split('.')[1])
                    )

                    const email =
                        payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress']
                        || payload.email
                        || 'User'

                    setUserEmail(email)
                } catch (err) {
                    console.error('Failed to decode JWT:', err)
                    localStorage.removeItem('token')
                    navigate('/login', { replace: true })
                    return
                }
            } catch (err) {
                console.error('Failed to validate authentication:', err)
                setError('Unable to connect to the server.')
            } finally {
                setLoading(false)
            }
        }

        validateAuthentication()
    }, [navigate])

    if (loading) {
        return (
            <div className="flex min-h-screen items-center justify-center bg-slate-50 px-4">
                <div className="text-center">
                    <div className="inline-block h-8 w-8 animate-spin rounded-full border-4 border-solid border-indigo-600 border-r-transparent" />
                </div>
            </div>
        )
    }

    if (error) {
        return (
            <div className="flex min-h-screen items-center justify-center bg-slate-50 px-4">
                <div className="w-full max-w-md rounded-2xl bg-white p-8 shadow-lg">
                    <div className="text-center">
                        <h1 className="text-xl font-semibold text-red-600">
                            Error
                        </h1>

                        <p className="mt-2 text-slate-600">
                            {error}
                        </p>

                        <button
                            onClick={() => window.location.reload()}
                            className="mt-6 rounded-lg bg-indigo-600 px-4 py-2 text-white hover:bg-indigo-700"
                        >
                            Retry
                        </button>
                    </div>
                </div>
            </div>
        )
    }

    return (
        <div className="flex min-h-screen items-center justify-center bg-slate-50 px-4">
            <div className="w-full max-w-md rounded-2xl bg-white p-8 shadow-lg">
                <div className="text-center">

                    <div className="mx-auto mb-4 flex h-16 w-16 items-center justify-center rounded-full bg-indigo-100">
                        <svg
                            className="h-8 w-8 text-indigo-600"
                            fill="none"
                            stroke="currentColor"
                            viewBox="0 0 24 24"
                        >
                            <path
                                strokeLinecap="round"
                                strokeLinejoin="round"
                                strokeWidth="2"
                                d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z"
                            />
                        </svg>
                    </div>

                    <h1 className="text-2xl font-semibold text-slate-900">
                        Hello
                    </h1>

                    <p className="mt-2 text-lg text-slate-600">
                        {userEmail}
                    </p>

                    <div className="mt-6 border-t border-slate-200" />

                    <p className="mt-4 text-sm text-slate-500">
                        Welcome Back
                    </p>
                </div>
            </div>
        </div>
    )
}

