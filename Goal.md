# Studi Kasus Terbaru: NexaDesk Helpdesk/Ticketing System

## 1. Profil perusahaan

**PT Arunika Distribusi Nusantara** merupakan perusahaan distribusi barang konsumsi dengan:

- Sekitar 800 karyawan
- 1 kantor pusat dan 12 cabang
- Beberapa gudang distribusi
- 25 anggota tim IT
- Operasional yang bergantung pada ERP, WMS, VPN, email, jaringan, dan perangkat kantor

Tim IT terdiri dari:

- Service Desk
- Application Support
- Network Support
- Infrastructure
- Information Security
- IT Asset Management

Gangguan sistem dapat berdampak langsung terhadap penjualan dan distribusi. Sebagai contoh, ERP yang tidak dapat diakses menyebabkan sales gagal memasukkan pesanan, sedangkan printer gudang yang bermasalah menghambat pencetakan dokumen pengiriman.

---

## 2. Kondisi sebelum sistem dibuat

Laporan masalah disampaikan melalui:

- WhatsApp pribadi teknisi
- Grup WhatsApp
- Email
- Telepon
- Microsoft Teams
- Penyampaian langsung
- Spreadsheet bersama

Belum ada satu sistem terpusat untuk mencatat dan memantau laporan.

### Alur sebelumnya

```text
Pengguna mengalami masalah
→ Mencari kontak anggota IT
→ Mengirim pesan melalui WhatsApp/email
→ Teknisi meminta informasi tambahan
→ Laporan diteruskan secara manual
→ Menunggu seseorang mengambil tanggung jawab
→ Pengguna melakukan follow-up
→ Masalah diselesaikan
→ Solusi tidak terdokumentasi
```

---

## 3. Permasalahan perusahaan

### Laporan mudah hilang

Laporan dapat tertumpuk, terlupakan, atau dikirim kepada teknisi yang sedang tidak bekerja.

### Informasi laporan tidak lengkap

Pesan seperti berikut sering dikirim:

> “ERP error, tolong segera diperbaiki.”

Pesan tersebut tidak menjelaskan lokasi, perangkat, pesan error, jumlah pengguna terdampak, dan waktu mulai gangguan.

### Tidak ada penanggung jawab yang jelas

Satu laporan dapat dikerjakan dua teknisi sekaligus atau tidak ditangani karena semua anggota tim menganggap orang lain sedang mengerjakannya.

### Prioritas tidak konsisten

Semua pengguna menganggap masalahnya mendesak. Tim IT belum memiliki matriks impact dan urgency untuk menentukan prioritas.

### Tidak ada target layanan

Perusahaan belum dapat mengukur:

- Waktu respons pertama
- Waktu penyelesaian
- Jumlah tiket terlambat
- Kepatuhan SLA
- Backlog
- Workload agent

### Pengguna tidak mengetahui status

Pengguna harus terus menghubungi tim IT untuk mengetahui apakah masalahnya sudah diterima, siapa yang menangani, dan kapan selesai.

### Pengetahuan tidak terdokumentasi

Solusi hanya tersimpan dalam percakapan pribadi atau ingatan teknisi. Ketika masalah yang sama muncul, investigasi dilakukan dari awal.

### Tidak tersedia audit trail

Perusahaan tidak memiliki catatan yang jelas mengenai siapa yang mengubah status, prioritas, assignment, atau konfigurasi layanan.

---

## 4. Dampak bisnis

Permasalahan tersebut menyebabkan:

- Proses pesanan dan distribusi terlambat
- Produktivitas karyawan menurun
- Tim IT sering terganggu chat pribadi
- Beban kerja agent tidak merata
- Masalah penting terlambat ditangani
- Kepercayaan pengguna terhadap tim IT menurun
- Manajemen tidak memiliki data untuk evaluasi
- Masalah berulang tidak dapat dianalisis
- Audit internal sulit dilakukan

---

## 5. Solusi yang dibangun

Perusahaan membangun **NexaDesk**, sebuah Helpdesk/Ticketing System terpusat untuk:

1. Mencatat seluruh laporan dalam satu sistem.
2. Memberikan nomor unik untuk setiap tiket.
3. Mengarahkan tiket ke tim yang tepat.
4. Menentukan prioritas berdasarkan dampak dan urgensi.
5. Mengukur response dan resolution SLA.
6. Memberikan status yang transparan kepada requester.
7. Menyimpan komunikasi dan riwayat perubahan.
8. Menjalankan eskalasi otomatis.
9. Mendokumentasikan solusi.
10. Menyediakan dashboard bagi agent dan manajemen.

---

## 6. Aktor sistem

### Requester

- Membuat tiket
- Melihat tiket sendiri
- Menambahkan komentar dan lampiran
- Mengonfirmasi solusi
- Membuka kembali tiket
- Memberikan rating

### Agent

- Melihat tiket tim
- Mengambil dan menangani tiket
- Memberikan komentar publik
- Menambahkan catatan internal
- Mengubah status
- Memberikan solusi

### Team Lead

- Menugaskan agent
- Memindahkan tiket
- Memantau workload
- Menangani eskalasi
- Melihat laporan tim

### Administrator

- Mengelola pengguna dan permission
- Mengelola tim dan kategori
- Mengatur SLA
- Mengatur kalender kerja
- Mengatur template notifikasi

### Auditor

- Melihat histori tiket
- Melihat pelanggaran SLA
- Melihat audit log
- Tidak dapat mengubah data operasional

---

## 7. Jenis tiket

### Incident

Gangguan terhadap layanan yang sebelumnya dapat digunakan.

Contoh:

- ERP tidak dapat diakses
- VPN gagal terhubung
- Printer gudang tidak berfungsi
- Jaringan kantor terputus

### Service Request

Permintaan layanan yang bukan merupakan gangguan.

Contoh:

- Instalasi aplikasi
- Pembuatan akun
- Permintaan akses folder
- Pengadaan perangkat

Versi pertama fokus pada `Incident` dan `ServiceRequest`.

---

## 8. Alur utama setelah sistem diterapkan

Contoh kasus: staf cabang tidak dapat menggunakan VPN.

```text
Requester membuat tiket
→ Sistem menghasilkan nomor tiket
→ Sistem menghitung prioritas
→ Sistem memilih SLA
→ Tiket diarahkan ke Network Support
→ Agent mengambil tiket
→ Agent memberikan respons pertama
→ Agent melakukan investigasi
→ Masalah diselesaikan
→ Requester mengonfirmasi solusi
→ Tiket ditutup
→ Data masuk ke dashboard
```

Contoh tiket:

```text
Nomor       : INC-2026-000184
Tipe        : Incident
Kategori    : Network
Subkategori : VPN
Impact      : Medium
Urgency     : High
Priority    : High
Status      : New
Assigned Team: Network Support
```

---

## 9. Workflow tiket

```text
New
→ Open
→ InProgress
→ Resolved
→ Closed
```

Alur tambahan:

```text
InProgress → PendingRequester → InProgress
InProgress → PendingVendor → InProgress
Resolved → Reopened → InProgress
New/Open → Cancelled
```

Aturan utamanya:

- Perubahan status hanya melalui transisi yang diizinkan.
- Requester tidak dapat menandai tiket sebagai `Resolved`.
- Hanya tiket `Resolved` yang dapat ditutup.
- Tiket dapat dibuka kembali dalam periode tertentu.
- Setiap perubahan status wajib memiliki histori.

---

## 10. Category dan automatic routing

Contoh kategori:

```text
Information Technology
├── Account
│   ├── Reset Password
│   └── Access Request
├── Hardware
│   ├── Laptop
│   └── Printer
├── Software
│   ├── Installation
│   └── Application Error
└── Network
    ├── Wi-Fi
    └── VPN
```

Kategori menentukan:

- Assigned team
- SLA policy
- Default priority
- Form tambahan
- Kebutuhan approval

Contoh:

```text
Network/VPN → Network Support
Software/ERP → Application Support
Hardware/Printer → IT Support
Account/Access Request → Service Desk
```

---

## 11. Priority matrix

Prioritas dihitung dari `Impact` dan `Urgency`.

| Impact | Urgency | Priority |
|---|---|---|
| High | High | Critical |
| High | Medium | High |
| Medium | High | High |
| Medium | Medium | Medium |
| Low | High | Medium |
| Low | Low | Low |

Contoh:

- Satu pengguna gagal login: `Low/Medium`
- Satu cabang tidak dapat mengakses ERP: `High/High`
- Permintaan instalasi aplikasi: `Low/Low`
- Semua gudang kehilangan jaringan: `High/High`

Perubahan priority oleh agent harus disertai alasan.

---

## 12. Assignment

Strategi assignment versi pertama:

1. Sistem mengarahkan tiket ke tim berdasarkan kategori.
2. Supervisor dapat menunjuk agent.
3. Agent dapat mengambil tiket yang belum ditugaskan.
4. Hanya agent dari tim terkait yang dapat mengambil tiket.
5. Satu tiket hanya memiliki satu agent utama.

Optimistic concurrency digunakan agar dua agent tidak berhasil mengambil tiket yang sama secara bersamaan.

---

## 13. SLA

Setiap tiket memiliki dua target:

- **First Response SLA:** waktu sampai respons pertama agent.
- **Resolution SLA:** waktu sampai tiket diselesaikan.

Contoh:

| Priority | Response | Resolution |
|---|---:|---:|
| Critical | 15 menit | 2 jam |
| High | 30 menit | 4 jam |
| Medium | 2 jam | 1 hari kerja |
| Low | 4 jam | 3 hari kerja |

Perhitungan SLA mempertimbangkan:

- Jam kerja
- Hari kerja
- Hari libur
- Zona waktu
- Status yang menghentikan SLA

Contoh jam kerja:

```text
Senin–Jumat
08.00–17.00
Asia/Jakarta
```

Ketika status menjadi `PendingRequester`, resolution SLA dapat dihentikan. SLA dilanjutkan ketika requester memberikan balasan.

---

## 14. Eskalasi otomatis

Background worker memeriksa penggunaan SLA secara berkala.

```text
75% SLA terpakai
→ Notifikasi kepada agent

90% SLA terpakai
→ Notifikasi kepada agent dan team lead

100% SLA terpakai
→ Tiket ditandai SLA breached
→ Eskalasi kepada IT Manager
→ Escalation event dicatat
```

Setiap tingkat eskalasi hanya dapat dijalankan satu kali agar notifikasi tidak terkirim berulang.

---

## 15. Komentar dan komunikasi

Sistem memiliki dua jenis komentar.

### Public comment

Dapat dilihat requester dan tim IT.

### Internal note

Hanya dapat dilihat agent dan supervisor.

Aturannya:

- Requester tidak boleh membuat atau membaca internal note.
- Komentar mencatat author dan waktu.
- Perubahan komentar memiliki histori.
- Lampiran hanya dapat diakses pengguna yang memiliki akses ke tiket.

---

## 16. Approval

Service request tertentu membutuhkan persetujuan.

Contoh:

```text
Requester meminta akses folder keuangan
→ Manager requester memberikan persetujuan
→ Data owner memberikan persetujuan
→ Tim IT memberikan akses
→ Tiket diselesaikan
```

Untuk versi pertama, cukup implementasikan satu approver. Approval bertingkat dapat dimasukkan ke fase berikutnya.

---

## 17. Knowledge base

Solusi tiket dapat diubah menjadi artikel knowledge base.

Contoh artikel:

- Cara reset password
- Cara menghubungkan VPN
- Troubleshooting printer
- Panduan instalasi aplikasi
- Cara meminta akses sistem

Manfaatnya:

- Requester dapat mencoba solusi mandiri.
- Agent tidak mengulang jawaban yang sama.
- Pengetahuan tidak hilang ketika agent resign.
- Waktu penyelesaian tiket berkurang.

---

## 18. Dashboard dan laporan

### Dashboard agent

- Tiket milik saya
- Tiket belum ditugaskan
- Tiket mendekati SLA
- Tiket melewati SLA
- Tiket berdasarkan priority

### Dashboard supervisor

- Jumlah tiket masuk dan selesai
- Backlog
- SLA compliance
- Average first response time
- Average resolution time
- Reopen rate
- Workload setiap agent
- Customer satisfaction score

### Laporan manajemen

- Tiket berdasarkan kategori
- Tiket berdasarkan cabang
- Gangguan paling sering terjadi
- Performa tim dan agent
- Pelanggaran SLA
- Tren tiket bulanan

---

## 19. Entitas utama

```text
User
Role
Permission
Team
TeamMember
Ticket
TicketType
TicketCategory
TicketAssignment
TicketStatusHistory
TicketComment
TicketCommentHistory
TicketAttachment
TicketWatcher
SlaPolicy
SlaInstance
BusinessCalendar
Holiday
EscalationRule
EscalationEvent
Approval
Notification
KnowledgeArticle
AuditLog
```

---

## 20. Arsitektur .NET

Gunakan modular monolith agar realistis untuk portofolio dan tetap mudah dikembangkan.

```text
ASP.NET Core Web API
.NET LTS
Entity Framework Core
PostgreSQL atau SQL Server
JWT/OpenID Connect
Background Worker
OpenTelemetry
Structured Logging
Docker
CI/CD
```

Contoh modul:

```text
Identity
Ticketing
Assignment
ServiceLevel
Approvals
KnowledgeBase
Notifications
Reporting
Auditing
```

---

## 21. Scope MVP

Fitur versi pertama:

- Authentication dan authorization
- User, role, permission, dan team
- Pengelolaan category
- Membuat dan melihat tiket
- Assignment team dan agent
- Workflow status
- Public comment dan internal note
- Priority matrix
- Response dan resolution SLA
- Business calendar
- Eskalasi otomatis
- Notification dalam aplikasi
- Audit log
- Dashboard dasar
- Unit dan integration test
- Docker Compose
- OpenAPI documentation

### Out-of-scope MVP

- Integrasi WhatsApp
- Tiket melalui email
- Chatbot
- AI classification
- Mobile application
- Multi-tenancy
- Problem management
- Change management
- Integrasi Microsoft Teams
- Approval bertingkat

---

## 22. Indikator keberhasilan

Implementasi dinilai berhasil apabila:

- Minimal 95% laporan IT dibuat melalui NexaDesk.
- Semua tiket aktif memiliki assigned team.
- First response SLA compliance mencapai minimal 90%.
- Resolution SLA compliance mencapai minimal 85%.
- Jumlah laporan melalui chat pribadi berkurang.
- Pengguna dapat memantau status tanpa menghubungi tim IT.
- Seluruh perubahan penting memiliki audit trail.
- Masalah berulang dapat diketahui melalui laporan.
- Solusi umum tersedia dalam knowledge base.

---

## 23. Nilai utama untuk portofolio

Proyek ini tidak hanya menunjukkan kemampuan CRUD, tetapi juga:

- Permission dan resource ownership
- State-machine workflow
- SLA berdasarkan kalender kerja
- Pause dan resume SLA
- Background worker
- Automatic escalation
- Concurrency control
- Idempotency
- Outbox pattern
- Attachment security
- Audit logging
- Reporting
- Automated testing
- Observability
- Deployment

Fokus implementasi terbaik adalah menyelesaikan satu alur tiket secara end-to-end—mulai dari pembuatan tiket, routing, assignment, SLA, komunikasi, penyelesaian, audit, sampai laporan—sebelum menambahkan fitur lanjutan.