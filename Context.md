# Konteks Perusahaan: Helpdesk/Ticketing System

## 1. Profil perusahaan

**Nama perusahaan:** PT Arunika Distribusi Nusantara  
**Bidang:** Distribusi barang konsumsi  
**Jumlah karyawan:** Sekitar 800 orang  
**Lokasi:** Kantor pusat dan 12 kantor cabang  
**Tim IT:** 25 orang, terdiri dari:

- Service Desk
- Infrastructure
- Network
- Application Support
- Information Security
- IT Asset Management

Perusahaan menggunakan berbagai sistem untuk kegiatan operasional:

- ERP untuk penjualan dan keuangan
- Warehouse Management System
- Aplikasi HR
- VPN
- Email perusahaan
- Microsoft 365
- Komputer dan printer di setiap cabang
- Jaringan kantor dan gudang

Operasional perusahaan sangat bergantung pada sistem tersebut. Gangguan kecil seperti printer gudang tidak berfungsi dapat menghambat pencetakan dokumen pengiriman. Gangguan besar seperti ERP tidak dapat diakses dapat menghentikan pemrosesan pesanan.

---

# 2. Kondisi sebelum ada sistem helpdesk

Sebelum menggunakan Helpdesk/Ticketing System, laporan masalah disampaikan melalui berbagai media:

- WhatsApp pribadi staf IT
- Grup WhatsApp kantor
- Telepon
- Email
- Chat Microsoft Teams
- Pesan langsung kepada teknisi
- Penyampaian secara lisan
- Spreadsheet bersama

Contoh situasi:

> Seorang staf cabang tidak dapat terhubung ke VPN. Ia mengirim pesan kepada salah satu teknisi melalui WhatsApp. Teknisi sedang cuti, tetapi pengguna tidak mengetahuinya. Laporan tersebut baru dibaca keesokan harinya.

Tidak ada satu tempat terpusat untuk mencatat dan memantau seluruh laporan.

---

# 3. Masalah yang dihadapi perusahaan

## Laporan mudah terlewat

Karena laporan masuk melalui banyak channel, tim IT sering lupa atau kehilangan informasi masalah.

Contoh:

- Pesan WhatsApp tertumpuk.
- Email masuk ke folder spam.
- Masalah disampaikan kepada teknisi yang sedang cuti.
- Laporan lisan tidak pernah dicatat.
- Pengguna menganggap laporannya sedang diproses, padahal belum diterima tim yang tepat.

## Pengguna tidak mengetahui status laporan

Setelah melapor, pengguna harus bertanya berulang kali:

> “Masalah saya sudah diproses belum?”  
> “Siapa yang menangani?”  
> “Kapan kira-kira selesai?”

Pengguna tidak mempunyai halaman untuk melihat status, riwayat komunikasi, atau estimasi penyelesaian.

## Tidak ada prioritas yang konsisten

Hampir semua pengguna menganggap masalah mereka mendesak. Tim IT menentukan prioritas secara subjektif.

Contoh:

- Permintaan instalasi aplikasi diberi prioritas tinggi.
- Gangguan ERP yang memengaruhi satu gudang terlambat ditangani.
- Permintaan reset password dan server production down masuk ke antrean yang sama.

Akibatnya, tim IT tidak selalu mengerjakan masalah berdasarkan dampak bisnis.

## Penugasan tidak jelas

Tidak ada aturan yang menentukan tim atau agent yang harus menangani laporan.

Contoh:

- Masalah VPN dikirim ke Application Support.
- Masalah ERP dikirim ke tim Network.
- Satu laporan dikerjakan oleh dua teknisi sekaligus.
- Semua teknisi mengira laporan sedang ditangani orang lain.
- Tiket terus berpindah tanpa ada pemilik yang jelas.

## Tidak ada target layanan

Perusahaan belum memiliki mekanisme untuk mengukur:

- Waktu respons pertama
- Waktu penyelesaian
- Jumlah laporan yang terlambat
- Jumlah laporan yang belum selesai
- Kepatuhan terhadap SLA
- Beban kerja setiap agent

Manajemen hanya mengetahui adanya masalah ketika pengguna melakukan komplain atau menghubungi manager IT.

## Riwayat penyelesaian tidak terdokumentasi

Solusi biasanya tersimpan di chat pribadi atau hanya diketahui oleh teknisi tertentu.

Akibatnya:

- Masalah yang sama dianalisis dari awal.
- Pengetahuan hilang ketika teknisi resign.
- Agent baru sulit mempelajari masalah sebelumnya.
- Tidak tersedia knowledge base untuk pengguna.

## Tidak tersedia data untuk pengambilan keputusan

Manajemen tidak dapat menjawab pertanyaan seperti:

- Berapa jumlah gangguan setiap bulan?
- Cabang mana yang paling sering mengalami masalah?
- Sistem mana yang paling banyak bermasalah?
- Berapa rata-rata waktu penyelesaian?
- Apakah jumlah agent sudah mencukupi?
- Berapa persen tiket memenuhi SLA?
- Apa penyebab gangguan yang paling sering terjadi?

Karena tidak ada data yang konsisten, keputusan biasanya dibuat berdasarkan keluhan terakhir atau asumsi.

---

# 4. Alur kerja sebelum ada sistem

## Contoh kasus: pengguna tidak dapat mengakses VPN

### Langkah 1 — Pengguna menemukan masalah

Seorang staf sales di cabang Bandung tidak dapat terhubung ke VPN. Karena VPN tidak tersedia, ia tidak dapat mengakses ERP untuk memasukkan pesanan pelanggan.

### Langkah 2 — Pengguna mencari orang IT

Pengguna tidak mengetahui siapa yang bertanggung jawab atas VPN. Ia bertanya kepada rekan kerja dan mendapatkan nomor salah satu teknisi IT.

### Langkah 3 — Laporan dikirim melalui WhatsApp

Pengguna mengirim pesan:

> “Mas, VPN saya tidak bisa. Tolong dicek, urgent.”

Informasi tersebut tidak lengkap. Tidak ada informasi tentang:

- Nama perangkat
- Lokasi pengguna
- Pesan error
- Waktu mulai gangguan
- Jumlah pengguna terdampak
- Screenshot
- Langkah yang sudah dicoba

### Langkah 4 — Teknisi meminta informasi tambahan

Teknisi membalas beberapa jam kemudian dan meminta screenshot. Pengguna sedang rapat sehingga baru membalas setelah satu jam.

Seluruh komunikasi berlangsung melalui chat pribadi dan tidak dapat dilihat anggota tim lainnya.

### Langkah 5 — Laporan diteruskan secara manual

Teknisi pertama mengetahui bahwa masalah tersebut merupakan tanggung jawab tim Network. Ia meneruskan screenshot ke grup Network.

Konteks masalah tidak diteruskan secara lengkap sehingga tim Network kembali menanyakan informasi yang sama.

### Langkah 6 — Tidak ada penanggung jawab

Beberapa anggota tim membaca pesan tersebut, tetapi tidak ada yang secara resmi ditugaskan. Masing-masing mengira teknisi lain sedang menanganinya.

### Langkah 7 — Pengguna melakukan eskalasi manual

Setelah menunggu beberapa jam, pengguna menghubungi supervisor. Supervisor kemudian menghubungi IT Manager.

Masalah baru mendapatkan perhatian setelah terjadi eskalasi melalui jalur manajemen.

### Langkah 8 — Masalah diselesaikan

Tim Network menemukan bahwa akun VPN pengguna terkunci dan kemudian membuka akun tersebut.

Solusi disampaikan melalui WhatsApp:

> “Sudah dicoba lagi, ya.”

Tidak ada pencatatan mengenai:

- Penyebab masalah
- Waktu respons
- Waktu penyelesaian
- Teknisi yang menyelesaikan
- Langkah penyelesaian
- Apakah masalah telah dikonfirmasi pengguna

### Langkah 9 — Masalah yang sama terulang

Beberapa minggu kemudian pengguna lain mengalami masalah serupa. Karena solusi sebelumnya tidak terdokumentasi, tim IT melakukan investigasi dari awal lagi.

---

# 5. Dampak terhadap perusahaan

## Dampak operasional

- Karyawan tidak dapat bekerja selama gangguan.
- Pesanan pelanggan terlambat diproses.
- Aktivitas gudang dapat terhenti.
- Permintaan akses membutuhkan waktu lama.
- Masalah yang sama berulang tanpa tindakan pencegahan.

## Dampak terhadap tim IT

- Agent sering terganggu oleh chat dan telepon.
- Pekerjaan sulit diprioritaskan.
- Pembagian beban kerja tidak merata.
- Banyak pekerjaan tidak tercatat.
- Evaluasi performa menjadi tidak objektif.
- Agent menghabiskan waktu menjawab pertanyaan status.

## Dampak terhadap pengguna

- Pengguna tidak mengetahui siapa yang menangani masalah.
- Pengguna harus menjelaskan masalah berulang kali.
- Tidak ada kepastian waktu respons.
- Kepercayaan terhadap tim IT menurun.
- Pengguna memilih menghubungi kenalan pribadi di tim IT.

## Dampak terhadap manajemen

- Tidak memiliki laporan kualitas layanan.
- Sulit menentukan kebutuhan jumlah agent.
- Sulit mengidentifikasi sistem yang sering bermasalah.
- Tidak memiliki bukti pencapaian SLA.
- Tidak tersedia audit trail untuk pemeriksaan internal.

---

# 6. Kebutuhan bisnis

Perusahaan kemudian memutuskan membangun **NexaDesk**, yaitu sistem helpdesk terpusat yang bertujuan untuk:

1. Menjadikan semua laporan tercatat dalam satu sistem.
2. Memberikan nomor referensi untuk setiap laporan.
3. Mengarahkan tiket secara otomatis ke tim yang tepat.
4. Menentukan prioritas berdasarkan impact dan urgency.
5. Menetapkan target respons dan penyelesaian melalui SLA.
6. Memberikan visibilitas status kepada pengguna.
7. Menyimpan seluruh komunikasi dan perubahan tiket.
8. Mendeteksi tiket yang mendekati atau melewati SLA.
9. Mendokumentasikan solusi.
10. Menyediakan laporan untuk manajemen.

---

# 7. Alur setelah sistem diterapkan

Dengan kasus VPN yang sama, alurnya menjadi berikut.

## 1. Pengguna membuat tiket

Pengguna login dan memilih:

```text
Tipe       : Incident
Kategori   : Network
Subkategori: VPN
Impact     : Medium
Urgency    : High
Judul      : Tidak dapat terhubung ke VPN
```

Pengguna memasukkan deskripsi, pesan error, dan screenshot.

## 2. Sistem melakukan validasi

Sistem memastikan informasi wajib telah diisi dan file yang diunggah memenuhi ketentuan keamanan.

Setelah tiket dibuat, sistem menghasilkan nomor:

```text
INC-2026-000184
```

## 3. Sistem menentukan prioritas

Berdasarkan kombinasi impact dan urgency, sistem menetapkan prioritas `High`.

Pengguna tidak dapat menaikkan prioritas secara sembarangan tanpa alasan atau validasi agent.

## 4. Sistem melakukan routing

Karena kategorinya `Network/VPN`, tiket otomatis masuk ke antrean `Network Support`.

SLA untuk tiket berprioritas tinggi langsung dimulai.

```text
Target respons    : 30 menit kerja
Target penyelesaian: 4 jam kerja
```

## 5. Agent mengambil tiket

Agent Network melihat tiket pada antrean dan memilih **Take Ticket**. Sistem mencatat agent sebagai penanggung jawab.

Optimistic concurrency mencegah dua agent mengambil tiket yang sama secara bersamaan.

## 6. Agent memberikan respons pertama

Agent mengirim komentar publik:

> “Laporan sudah kami terima. Kami sedang memeriksa status akun dan koneksi VPN Anda.”

Sistem mencatat first response time dan menghentikan timer first-response SLA.

## 7. Agent meminta informasi tambahan

Jika agent membutuhkan informasi, status diubah menjadi `PendingRequester`.

Sistem:

- Memberi tahu requester
- Mencatat alasan pending
- Menghentikan sementara resolution SLA sesuai kebijakan

## 8. Pengguna memberikan jawaban

Ketika pengguna membalas, status otomatis kembali menjadi `InProgress` dan SLA dilanjutkan.

## 9. Agent menyelesaikan masalah

Agent membuka akun VPN yang terkunci dan mencatat:

- Penyebab masalah
- Langkah penyelesaian
- Tindakan pencegahan
- Apakah solusi dapat dijadikan artikel knowledge base

Status tiket berubah menjadi `Resolved`.

## 10. Pengguna mengonfirmasi

Pengguna mencoba kembali koneksi VPN lalu mengonfirmasi bahwa masalah selesai.

Status tiket berubah menjadi `Closed`, kemudian pengguna memberikan rating layanan.

## 11. Data masuk ke laporan

Sistem memperbarui:

- Jumlah tiket selesai
- Waktu respons pertama
- Waktu penyelesaian
- SLA compliance
- Workload agent
- Statistik masalah VPN
- Customer satisfaction score

---

# 8. Alur jika SLA hampir terlewati

Sistem menjalankan pemeriksaan berkala melalui background worker.

```text
75% SLA terpakai
→ Notifikasi kepada agent

90% SLA terpakai
→ Notifikasi kepada agent dan team lead

100% SLA terpakai
→ Tiket ditandai breached
→ Eskalasi kepada IT manager
→ Escalation event dicatat
```

Dengan mekanisme ini, eskalasi tidak lagi bergantung pada pengguna yang marah atau menghubungi manager secara pribadi.

---

# 9. Aturan bisnis utama

Beberapa aturan bisnis yang dapat digunakan dalam proyek:

### Pembuatan tiket

- Requester hanya boleh membuat tiket untuk dirinya sendiri, kecuali memiliki permission khusus.
- Tiket wajib memiliki judul, deskripsi, tipe, impact, dan urgency.
- Nomor tiket harus unik.
- Tiket baru tidak boleh langsung berstatus `Closed`.

### Assignment

- Agent hanya boleh mengambil tiket dari timnya.
- Supervisor dapat memindahkan tiket ke tim lain.
- Tiket aktif harus mempunyai assigned team.
- Satu tiket hanya boleh memiliki satu agent utama.
- Perubahan assignment harus dicatat.

### Status

- Status hanya boleh berubah melalui transisi yang diizinkan.
- Tiket hanya dapat diselesaikan oleh agent.
- Tiket hanya dapat ditutup setelah berstatus `Resolved`.
- Requester dapat membuka kembali tiket dalam batas waktu tertentu.
- Tiket yang sudah ditutup tidak dapat diedit tanpa permission khusus.

### SLA

- SLA ditentukan berdasarkan tipe, kategori, dan prioritas tiket.
- SLA menggunakan jam kerja departemen.
- Status tertentu dapat menghentikan SLA.
- SLA yang sudah dilanggar tetap tercatat meskipun tiket akhirnya selesai.
- Setiap tingkat eskalasi hanya boleh dijalankan satu kali.

### Komentar

- Requester hanya dapat melihat komentar publik.
- Internal note hanya dapat dilihat oleh agent dan supervisor.
- Komentar yang diedit harus memiliki riwayat.
- Semua komentar harus mencatat author dan waktu.

---

# 10. Scope versi pertama

Agar proyek portofolio tetap realistis, versi pertama dapat difokuskan pada:

- Login dan authorization
- Pengelolaan user, team, dan permission
- Kategori tiket
- Pembuatan tiket
- Daftar dan detail tiket
- Assignment ke team dan agent
- Perubahan status
- Public comment dan internal note
- Priority matrix
- SLA response dan resolution
- Business calendar
- Eskalasi otomatis
- Notification dalam aplikasi
- Audit log
- Dashboard dasar
- API documentation
- Unit dan integration test
- Docker deployment

## Di luar scope versi pertama

- Tiket melalui email
- Chatbot
- Integrasi WhatsApp
- Integrasi Microsoft Teams
- AI classification
- Mobile application
- Multi-tenancy
- Problem management
- Change management
- Approval bertingkat
- Integrasi monitoring server

Menuliskan out-of-scope menunjukkan bahwa Anda mampu mengendalikan batas proyek, bukan sekadar menambah fitur.

---

# 11. Indikator keberhasilan

Perusahaan menganggap implementasi berhasil jika:

- Minimal 95% laporan IT dibuat melalui NexaDesk.
- Tidak ada tiket yang tidak memiliki assigned team.
- First response SLA compliance mencapai minimal 90%.
- Resolution SLA compliance mencapai minimal 85%.
- Rata-rata waktu respons berkurang.
- Jumlah pertanyaan status melalui chat pribadi berkurang.
- Seluruh perubahan status dan assignment memiliki audit trail.
- Masalah berulang dapat diidentifikasi melalui laporan kategori.
- Pengguna dapat melihat status tanpa menghubungi tim IT.

Konteks ini membuat proyek portofolio tidak hanya terlihat sebagai aplikasi CRUD. Anda menunjukkan bahwa sistem dibuat untuk menyelesaikan masalah operasional nyata, memiliki aturan bisnis terukur, serta menghasilkan perubahan proses yang jelas sebelum dan sesudah implementasi.