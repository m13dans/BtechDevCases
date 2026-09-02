import { useState } from 'react'
import { useNavigate, Link } from 'react-router'

const API_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5000'

type FormState = {
    email: string
    password: string
    confirmPassword: string
}

export default function Register() {
    const navigate = useNavigate()
    const [form, setForm] = useState<FormState>({
        email: '',
        password: '',
        confirmPassword: '',
    })
    const [loading, setLoading] = useState(false)
    const [error, setError] = useState<string | null>(null)
    const [success, setSuccess] = useState(false)

    console.log('API_URL:', API_URL)

    const handleChange = (field: keyof FormState) => (e: React.ChangeEvent<HTMLInputElement>) => {
        setForm((prev) => ({ ...prev, [field]: e.target.value }))
    }

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault()
        setError(null)
        setSuccess(false)

        if (form.password !== form.confirmPassword) {
            setError('Password and confirm password do not match')
            return
        }

        setLoading(true)
        try {
            const res = await fetch(`${API_URL}/api/auth/register`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    email: form.email,
                    password: form.password,
                    confirmPassword: form.confirmPassword,
                }),
            })

            const data = await res.json()

            if (!res.ok || data.responseType !== 0) {
                throw new Error(data.message ?? 'Registration failed')
            }

            setSuccess(true)
            setForm({ email: '', password: '', confirmPassword: '' })

            // redirect ke login setelah sesaat
            setTimeout(() => navigate('/login'), 1500)
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Something went wrong')
        } finally {
            setLoading(false)
        }
    }

    return (
        <div className="flex min-h-screen items-center justify-center bg-slate-50 px-4">
            <div className="w-full max-w-md rounded-2xl bg-white p-8 shadow-lg">
                <h1 className="mb-1 text-2xl font-semibold text-slate-900">Create an account</h1>
                <p className="mb-6 text-sm text-slate-500">Sign up to get started</p>

                <form onSubmit={handleSubmit} className="flex flex-col gap-4">
                    <div className="flex flex-col gap-1">
                        <label htmlFor="email" className="text-sm font-medium text-slate-700">
                            Email
                        </label>
                        <input
                            id="email"
                            type="email"
                            required
                            value={form.email}
                            onChange={handleChange('email')}
                            className="rounded-lg border border-slate-300 px-3 py-2 text-sm text-slate-900 outline-none transition focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100"
                            placeholder="you@example.com"
                        />
                    </div>

                    <div className="flex flex-col gap-1">
                        <label htmlFor="password" className="text-sm font-medium text-slate-700">
                            Password
                        </label>
                        <input
                            id="password"
                            type="password"
                            required
                            minLength={4}
                            value={form.password}
                            onChange={handleChange('password')}
                            className="rounded-lg border border-slate-300 px-3 py-2 text-sm text-slate-900 outline-none transition focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100"
                            placeholder="••••"
                        />
                    </div>

                    <div className="flex flex-col gap-1">
                        <label htmlFor="confirm-password" className="text-sm font-medium text-slate-700">
                            Confirm Password
                        </label>
                        <input
                            id="confirm-password"
                            type="password"
                            required
                            minLength={4}
                            value={form.confirmPassword}
                            onChange={handleChange('confirmPassword')}
                            className="rounded-lg border border-slate-300 px-3 py-2 text-sm text-slate-900 outline-none transition focus:border-indigo-500 focus:ring-2 focus:ring-indigo-100"
                            placeholder="••••"
                        />
                    </div>

                    {error && (
                        <p className="rounded-lg bg-red-50 px-3 py-2 text-sm text-red-600">{error}</p>
                    )}
                    {success && (
                        <p className="rounded-lg bg-green-50 px-3 py-2 text-sm text-green-600">
                            Registered successfully! Redirecting to login...
                        </p>
                    )}

                    <button
                        type="submit"
                        disabled={loading}
                        className="mt-2 rounded-lg bg-indigo-600 px-4 py-2 text-sm font-semibold text-white transition hover:bg-indigo-700 disabled:cursor-not-allowed disabled:opacity-60"
                    >
                        {loading ? 'Registering...' : 'Register'}
                    </button>

                    <p className="text-center text-sm text-slate-500">
                        Already have an account?{' '}
                        <Link to="/login" className="font-medium text-indigo-600 hover:underline">
                            Log in
                        </Link>
                    </p>
                </form>
            </div>
        </div>
    )
}